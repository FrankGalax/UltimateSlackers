using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpotifyAPI;

namespace SpotifyAPI.Web
{
    public class SpotifyConnector
    {
        private String mSpotifyId = "b59d9e0a7019488eaf252f806f18ef22";
        private String mSpotifySecret = "5feb1f737c2944239893f56954b3634c";
        private SpotifyWebAPI mSpotify;
        private Auth.ImplicitGrantAuth mAuth;
        private static SpotifyConnector mInstance = null;
        private String mErrorMessage;
        private Models.PrivateProfile mProfile;
        private List<Models.SimplePlaylist> playlists;

        public static SpotifyConnector GetSpotifyConnector()
        {
            if (mInstance == null)
            {
                mInstance = new SpotifyConnector();
            }
            return mInstance;
        }

        public String Connect()
        {
            mErrorMessage = null;
            mAuth = new Auth.ImplicitGrantAuth
            {
                RedirectUri = "http://localhost:8000",
                ClientId = mSpotifyId,
                Scope = Enums.Scope.UserReadPrivate | Enums.Scope.UserReadEmail | Enums.Scope.PlaylistReadPrivate | Enums.Scope.UserLibraryRead | Enums.Scope.UserReadPrivate | Enums.Scope.UserFollowRead | Enums.Scope.UserReadBirthdate | Enums.Scope.UserTopRead | Enums.Scope.PlaylistModifyPrivate | Enums.Scope.PlaylistModifyPublic,
                State = "XSS"
            };
            mAuth.OnResponseReceivedEvent += authResponseReceivedEvent;

            mAuth.StartHttpServer(8000);
            mAuth.DoAuth();

            while (mSpotify == null) { };

            if (mErrorMessage == null)
            {
                return "Connection Successful";
            }
            else
            {
                return mErrorMessage;
            }
        }

        private void authResponseReceivedEvent(Models.Token token, string state)
        {
            mAuth.StopHttpServer();
            mSpotify = new SpotifyWebAPI();
            if (state != "XSS")
            {
                mErrorMessage = "Wrong state received.";
                return;
            }
            if (token.Error != null)
            {
                mErrorMessage = token.Error;
                return;
            }

            mSpotify.UseAuth = true;
            mSpotify.AccessToken = token.AccessToken;
            mSpotify.TokenType = token.TokenType;
            mProfile = mSpotify.GetPrivateProfile();
        }

        public String CreateNewPlaylist(String playlistName)
        {
            if (mProfile == null || mSpotify == null)
            {
                return "you need to connect to spotify before you can use this feature";
            }
            if (playlists == null)
            {
                playlists = mSpotify.GetUserPlaylists(mProfile.Id).Items;
            }
            if (playlists.Find(x => x.Name == playlistName) != null)
            {
               return $"A playlist named {playlistName} already exists";
            }

            Models.FullPlaylist newPlaylist = mSpotify.CreatePlaylist(mProfile.Id, playlistName, false);

            Models.SimplePlaylist newSimplePlaylist = new Models.SimplePlaylist();
            newSimplePlaylist.Name = newPlaylist.Name;
            newSimplePlaylist.Id = newPlaylist.Id;
            newSimplePlaylist.Owner = newPlaylist.Owner;
            playlists.Add(newSimplePlaylist);

            return $"{playlistName} was successfully created";
        }

        public String AddSongToPlayList(String playlistName, String songName, String artist)
        {
            if (mProfile == null || mSpotify == null)
            {
                return "you need to connect to spotify before you can use this feature";
            }

            if (playlists == null)
            {
                playlists = mSpotify.GetUserPlaylists(mProfile.Id).Items;
            }

            Models.SimplePlaylist playlist = playlists.Find(x => x.Name == playlistName);

            if (playlist == null)
            {
                return $"could not find playlist {playlistName}";
            }

            Models.SearchItem search = mSpotify.SearchItems(songName, Enums.SearchType.Track);

            if (search.Tracks == null || search.Tracks.Items.Count == 0)
            {
                return $"could not find song {songName}";
            }

            Models.FullTrack songData = search.Tracks.Items.Find(x => String.Equals(x.Artists[0].Name, artist, StringComparison.OrdinalIgnoreCase));

            if (songData == null)
            {
                return $"could not find song {songName} by {artist}";
            }

            Models.ErrorResponse error = mSpotify.AddPlaylistTrack(mProfile.Id, playlist.Id, songData.Uri);

            if (error.Error != null)
            {
                return error.Error.Message;
            }

            return $"{songName} by {artist} was sucessfully added to {playlistName}";
        }
    }
}