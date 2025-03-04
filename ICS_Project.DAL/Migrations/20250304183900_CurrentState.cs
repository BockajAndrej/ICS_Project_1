using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICS_Project.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CurrentState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArtistName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    GenreName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MusicTracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Length = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Size = table.Column<double>(type: "REAL", nullable: false),
                    UrlAddress = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicTracks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    NumberOfMusicTracks = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPlayTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArtistMusicTrack",
                columns: table => new
                {
                    ArtistsId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MusicTracksId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistMusicTrack", x => new { x.ArtistsId, x.MusicTracksId });
                    table.ForeignKey(
                        name: "FK_ArtistMusicTrack_Artists_ArtistsId",
                        column: x => x.ArtistsId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArtistMusicTrack_MusicTracks_MusicTracksId",
                        column: x => x.MusicTracksId,
                        principalTable: "MusicTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenreMusicTrack",
                columns: table => new
                {
                    GenresId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MusicTracksId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreMusicTrack", x => new { x.GenresId, x.MusicTracksId });
                    table.ForeignKey(
                        name: "FK_GenreMusicTrack_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreMusicTrack_MusicTracks_MusicTracksId",
                        column: x => x.MusicTracksId,
                        principalTable: "MusicTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MusicTrackPlaylist",
                columns: table => new
                {
                    MusicTracksId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PlaylistsId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicTrackPlaylist", x => new { x.MusicTracksId, x.PlaylistsId });
                    table.ForeignKey(
                        name: "FK_MusicTrackPlaylist_MusicTracks_MusicTracksId",
                        column: x => x.MusicTracksId,
                        principalTable: "MusicTracks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MusicTrackPlaylist_Playlists_PlaylistsId",
                        column: x => x.PlaylistsId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistMusicTrack_MusicTracksId",
                table: "ArtistMusicTrack",
                column: "MusicTracksId");

            migrationBuilder.CreateIndex(
                name: "IX_GenreMusicTrack_MusicTracksId",
                table: "GenreMusicTrack",
                column: "MusicTracksId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicTrackPlaylist_PlaylistsId",
                table: "MusicTrackPlaylist",
                column: "PlaylistsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistMusicTrack");

            migrationBuilder.DropTable(
                name: "GenreMusicTrack");

            migrationBuilder.DropTable(
                name: "MusicTrackPlaylist");

            migrationBuilder.DropTable(
                name: "Artists");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "MusicTracks");

            migrationBuilder.DropTable(
                name: "Playlists");
        }
    }
}