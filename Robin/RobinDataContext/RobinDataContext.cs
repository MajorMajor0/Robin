using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Robin
{
	public partial class RobinDataContext : DbContext
	{
		public RobinDataContext()
		{
		}

		public virtual DbSet<Collection> Collections { get; set; }
		public virtual DbSet<Core> Cores { get; set; }
		public virtual DbSet<Emulator> Emulators { get; set; }
		public virtual DbSet<Game> Games { get; set; }
		public virtual DbSet<GBGame> GBGames { get; set; }
		public virtual DbSet<GBPlatform> GBPlatforms { get; set; }
		public virtual DbSet<GBRelease> GBReleases { get; set; }
		public virtual DbSet<GDBPlatform> GDBPlatforms { get; set; }
		public virtual DbSet<GDBRelease> GDBReleases { get; set; }
		public virtual DbSet<Language> Languages { get; set; }
		public virtual DbSet<LBGame> LBGames { get; set; }
		public virtual DbSet<LBImage> LBImages { get; set; }
		public virtual DbSet<LBPlatform> LBPlatforms { get; set; }
		public virtual DbSet<LBRelease> LBReleases { get; set; }
		public virtual DbSet<Match> Matches { get; set; }
		public virtual DbSet<MBGame> MBGames { get; set; }
		public virtual DbSet<MBGenre> MBGenres { get; set; }
		public virtual DbSet<MBPlatform> MBPlatforms { get; set; }
		public virtual DbSet<MBRelease> Mbreleases { get; set; }
		public virtual DbSet<OVGPlatform> OVGPlatforms { get; set; }
		public virtual DbSet<OVGRelease> OVGReleases { get; set; }
		public virtual DbSet<Platform> Platforms { get; set; }
		public virtual DbSet<Region> Regions { get; set; }
		public virtual DbSet<Release> Releases { get; set; }
		public virtual DbSet<Rom> Roms { get; set; }
		public virtual DbSet<URL> Urls { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
				optionsBuilder.UseSqlite($"DataSource={R.FileLocation}");

				//string path = @"C:\Robin_debug\data\RobinData.db3";
				//optionsBuilder.UseSqlite($"DataSource={path}");

				//	optionsBuilder.UseModel(RobinDataContextModel.Instance);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Collection>(entity =>
			{
				entity.ToTable("Collection");

				entity.HasIndex(e => e.ID, "IX_Collection_ID")
					.IsUnique();

				entity.Property(e => e.ID).HasColumnName("ID");

				entity.Property(e => e.Title).IsRequired();

				entity.Property(e => e.Type)
					.IsRequired()
					.HasDefaultValueSql("Game");

				entity.HasMany(d => d.Games)
					.WithMany(p => p.Collections)
					.UsingEntity<Dictionary<string, object>>(
						"CollectionGame",
						l => l.HasOne<Game>().WithMany().HasForeignKey("GameId"),
						r => r.HasOne<Collection>().WithMany().HasForeignKey("CollectionId"),
						j =>
						{
							j.HasKey("CollectionId", "GameId");

							j.ToTable("Collection_Game");

							j.HasIndex(new[] { "CollectionId", "GameId" }, "CG");

							j.IndexerProperty<long>("CollectionId").HasColumnName("Collection_ID");

							j.IndexerProperty<long>("GameId").HasColumnName("Game_ID");
						});

				entity.HasMany(d => d.Releases)
					.WithMany(p => p.Collections)
					.UsingEntity<Dictionary<string, object>>(
						"CollectionRelease",
						l => l.HasOne<Release>().WithMany().HasForeignKey("ReleaseId").OnDelete(DeleteBehavior.ClientSetNull),
						r => r.HasOne<Collection>().WithMany().HasForeignKey("CollectionId"),
						j =>
						{
							j.HasKey("CollectionId", "ReleaseId");

							j.ToTable("Collection_Release");

							j.HasIndex(new[] { "CollectionId", "ReleaseId" }, "CR");

							j.IndexerProperty<long>("CollectionId").HasColumnName("Collection_ID");

							j.IndexerProperty<long>("ReleaseId").HasColumnName("Release_ID");
						});
			});

			modelBuilder.Entity<Core>(entity =>
			{
				entity.ToTable("Core");

				entity.HasIndex(e => e.ID, "IX_Core_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.Emulator_ID).HasColumnName("Emulator_ID");

				entity.Property(e => e.FileName).IsRequired();

				entity.Property(e => e.Title).IsRequired();

				entity.HasOne(d => d.Emulator)
					.WithMany(p => p.Cores)
					.HasForeignKey(d => d.Emulator_ID);

				entity.HasMany(d => d.Platforms)
					.WithMany(p => p.Cores)
					.UsingEntity<Dictionary<string, object>>(
						"CorePlatform",
						l => l.HasOne<Platform>().WithMany().HasForeignKey("PlatformId"),
						r => r.HasOne<Core>().WithMany().HasForeignKey("CoreId"),
						j =>
						{
							j.HasKey("CoreId", "PlatformId");

							j.ToTable("Core_Platform");

							j.IndexerProperty<long>("CoreId").HasColumnName("Core_ID");

							j.IndexerProperty<long>("PlatformId").HasColumnName("Platform_ID");
						});
			});

			modelBuilder.Entity<Emulator>(entity =>
			{
				entity.ToTable("Emulator");

				entity.HasIndex(e => e.ID, "IX_Emulator_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.IsCrap)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");

				entity.Property(e => e.Rating).HasColumnType("NUMERIC");

				entity.Property(e => e.Website).HasDefaultValueSql("'https://www.google.com'");
			});

			modelBuilder.Entity<Game>(entity =>
			{
				entity.ToTable("Game");

				entity.HasIndex(e => e.ID, "IX_Game_ID")
					.IsUnique();

				entity.HasIndex(e => e.ID, "GID");

				entity.Property(e => e.ID).HasColumnName("ID");

				entity.Property(e => e.IsAdult)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");

				entity.Property(e => e.IsBeaten)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");

				entity.Property(e => e.IsCrap)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");

				entity.Property(e => e.IsGame)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("1");

				entity.Property(e => e.IsMess)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");

				entity.Property(e => e.Rating).HasColumnType("NUMERIC");

				entity.Property(e => e.Unlicensed)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");
			});

			modelBuilder.Entity<GBGame>(entity =>
			{
				entity.ToTable("GBGame");

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.BoxFrontUrl).HasColumnName("BoxFrontUrl");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.GBPlatform_ID).HasColumnName("GBPlatform_ID");

				entity.Property(e => e.ScreenUrl).HasColumnName("ScreenUrl");

				entity.HasOne(d => d.GBPlatform)
					.WithMany(p => p.GBGames)
					.HasForeignKey(d => d.GBPlatform_ID)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<GBPlatform>(entity =>
			{
				entity.ToTable("GBPlatform");

				entity.HasIndex(e => e.ID, "IX_GBPlatform_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.CacheDate)
					.IsRequired()
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("1900 - 1 - 1 - 0 - 0 - 0");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<GBRelease>(entity =>
			{
				entity.ToTable("GBRelease");

				entity.HasIndex(e => e.ID, "IX_GBRelease_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.BoxUrl).HasColumnName("BoxUrl");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.GBGame_ID).HasColumnName("GBGame_ID");

				entity.Property(e => e.GBPlatform_ID).HasColumnName("GBPlatform_ID");

				entity.Property(e => e.Region_ID)
					.HasColumnName("Region_ID")
					.HasDefaultValueSql("0");

				entity.Property(e => e.ScreenUrl).HasColumnName("ScreenUrl");

				entity.Property(e => e.Title).IsRequired();

				entity.HasOne(d => d.GBGame)
					.WithMany(p => p.GBReleases)
					.HasForeignKey(d => d.GBGame_ID).HasAnnotation("Column", "GBGame_ID")
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.GBPlatform)
					.WithMany(p => p.GBReleases)
					.HasForeignKey(d => d.GBPlatform_ID);

				entity.HasOne(d => d.Region)
					.WithMany(p => p.GBReleases)
					.HasForeignKey(d => d.Region_ID);
			});

			modelBuilder.Entity<GDBPlatform>(entity =>
			{
				entity.ToTable("GDBPlatform");

				entity.HasIndex(e => e.ID, "IX_GDBPlatform_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.BannerUrl).HasColumnName("BannerUrl");

				entity.Property(e => e.BoxBackUrl).HasColumnName("BoxBackUrl");

				entity.Property(e => e.BoxFrontUrl).HasColumnName("BoxFrontUrl");

				entity.Property(e => e.CacheDate)
					.IsRequired()
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("1900 - 1 - 1 - 0 - 0 - 0");

				entity.Property(e => e.ConsoleUrl).HasColumnName("ConsoleUrl");

				entity.Property(e => e.ControllerUrl).HasColumnName("ControllerUrl");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.Rating).HasColumnType("NUMERIC");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<GDBRelease>(entity =>
			{
				entity.ToTable("GDBRelease");

				entity.HasIndex(e => e.ID, "IX_GDBRelease_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.BannerUrl).HasColumnName("BannerUrl");

				entity.Property(e => e.BoxBackUrl).HasColumnName("BoxBackUrl");

				entity.Property(e => e.BoxFrontUrl).HasColumnName("BoxFrontUrl");

				entity.Property(e => e.Coop).HasColumnType("BOOLEAN");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.GDBPlatform_ID).HasColumnName("GDBPlatform_ID");

				entity.Property(e => e.LogoUrl).HasColumnName("LogoUrl");

				entity.Property(e => e.Rating).HasColumnType("NUMERIC");

				entity.Property(e => e.ScreenUrl).HasColumnName("ScreenUrl");

				entity.HasOne(d => d.GDBPlatform)
					.WithMany(p => p.GDBReleases)
					.HasForeignKey(d => d.GDBPlatform_ID);
			});

			modelBuilder.Entity<Language>(entity =>
			{
				entity.ToTable("Language");

				entity.HasIndex(e => e.Abbreviation, "IX_Language_Abbreviation")
					.IsUnique();

				entity.HasIndex(e => e.Title, "IX_Language_Title")
					.IsUnique();

				entity.Property(e => e.ID)
					.HasColumnType("integer")
					.HasColumnName("ID");

				entity.Property(e => e.Abbreviation)
					.IsRequired()
					.HasColumnType("text");

				entity.Property(e => e.Title)
					.IsRequired()
					.HasColumnType("text");
			});

			modelBuilder.Entity<LBGame>(entity =>
			{
				entity.ToTable("LBGame");

				entity.HasIndex(e => e.ID, "IX_LBGame_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.LBPlatform_ID).HasColumnName("LBPlatform_ID");

				entity.Property(e => e.Title).IsRequired();

				entity.Property(e => e.VideoUrl).HasColumnName("VideoUrl");

				entity.Property(e => e.WikiUrl).HasColumnName("WikiUrl");

				entity.HasOne(d => d.LBPlatform)
					.WithMany(p => p.LBGames)
					.HasForeignKey(d => d.LBPlatform_ID);
			});

			modelBuilder.Entity<LBImage>(entity =>
			{
				entity.ToTable("LBImages");

				entity.HasIndex(e => e.FileName, "IX_LBImages_FileName")
					.IsUnique();

				entity.HasIndex(e => e.ID, "IX_LBImages_ID")
					.IsUnique();

				entity.Property(e => e.ID).HasColumnName("ID");

				entity.Property(e => e.FileName)
					.IsRequired()
					.HasColumnType("STRING");

				entity.Property(e => e.LBRegion).HasColumnName("LBRegion");

				entity.Property(e => e.LBRelease_ID).HasColumnName("LBRelease_ID");

				entity.Property(e => e.Region_ID).HasColumnName("Region_ID");

				entity.Property(e => e.Type).IsRequired();

				entity.HasOne(d => d.LBRelease)
					.WithMany(p => p.LBImages)
					.HasForeignKey(d => d.LBRelease_ID)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<LBPlatform>(entity =>
			{
				entity.ToTable("LBPlatform");

				entity.HasIndex(e => e.ID, "IX_LBPlatform_ID")
					.IsUnique();

				entity.Property(e => e.ID).HasColumnName("ID");

				entity.Property(e => e.CacheDate)
					.IsRequired()
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("1900 - 1 - 1 - 0 - 0 - 0");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<LBRelease>(entity =>
			{
				entity.ToTable("LBRelease");

				entity.HasIndex(e => e.ID, "IX_LBRelease_ID")
					.IsUnique();

				entity.Property(e => e.ID).HasColumnName("ID");

				entity.Property(e => e.LBGame_ID).HasColumnName("LBGame_ID");

				entity.Property(e => e.Region_ID).HasColumnName("Region_ID");

				entity.HasOne(d => d.LBGame)
					.WithMany(p => p.LBReleases)
					.HasForeignKey(d => d.LBGame_ID);

				entity.HasOne(d => d.Region)
					.WithMany(p => p.LBReleases)
					.HasForeignKey(d => d.Region_ID)
					.OnDelete(DeleteBehavior.ClientSetNull);
			});

			modelBuilder.Entity<Match>(entity =>
			{
				entity.ToTable("Match");

				entity.HasIndex(e => e.ID, "IX_Match_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.ID_GB).HasColumnName("ID_GB");

				entity.Property(e => e.ID_GDB).HasColumnName("ID_GDB");

				entity.Property(e => e.ID_OVG).HasColumnName("ID_OVG");
				entity.Property(e => e.ID_LB).HasColumnName("ID_LB");
				entity.Property(e => e.ID_MB).HasColumnName("ID_MB");

				entity.Property(e => e.Region_ID)
					.HasColumnName("RegionId")
					.HasDefaultValueSql("0");

				entity.Property(e => e.SHA1)
					.IsRequired()
					.HasColumnName("Sha1");
			});

			modelBuilder.Entity<MBGame>(entity =>
			{
				entity.ToTable("MBGame");

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.MBPlatform_ID).HasColumnName("MBPlatform_ID");

				entity.HasOne(d => d.MBPlatform)
					.WithMany(p => p.MBGames)
					.HasForeignKey(d => d.MBPlatform_ID).HasAnnotation("Column", "MBPlatform_ID")
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasMany(d => d.MBGenres)
				   .WithMany(p => p.MBGames)
				   .UsingEntity<Dictionary<string, object>>(
					   "MBGenreMBGame",
					   l => l.HasOne<MBGenre>().WithMany().HasForeignKey("MBGenreId").HasAnnotation("Column", "MBGenre_ID"),
					   r => r.HasOne<MBGame>().WithMany().HasForeignKey("MBGameId").HasAnnotation("Column", "MBGame_ID"),
					   j =>
					   {
						   j.HasKey("MBGameId", "MBGenreId");

						   j.ToTable("MBGenre_MBGame");

						   j.IndexerProperty<long>("MBGameId").HasColumnName("MBGame_ID");

						   j.IndexerProperty<long>("MBGenreId").HasColumnName("MBGenre_ID");
					   });
			});

			modelBuilder.Entity<MBGenre>(entity =>
			{
				entity.ToTable("MBGenre");

				entity.HasIndex(e => e.ID, "IX_MBGenre_ID")
					.IsUnique();

				entity.Property(e => e.ID).ValueGeneratedNever();

				entity.Property(e => e.Description).IsRequired();

				entity.Property(e => e.Name).IsRequired();

				entity.HasOne(d => d.Category)
					.WithMany(p => p.MBGenres)
					.HasForeignKey(d => d.Category_ID);
			});

			modelBuilder.Entity<MBGenreCategory>(entity =>
			{
				entity.ToTable("MBGenreCategory");

				entity.HasIndex(e => e.Id, "IX_MBGenreCategory_Id")
					.IsUnique();

				entity.HasIndex(e => e.Name, "IX_MBGenreCategory_Name")
					.IsUnique();

				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Name).IsRequired();
			});
			modelBuilder.Entity<MBPlatform>(entity =>
			{
				entity.ToTable("MBPlatform");

				entity.HasIndex(e => e.ID, "IX_MBPlatform_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.CacheDate)
					.IsRequired()
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("1900 - 1 - 1 - 0 - 0 - 0");

				// entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<MBRelease>(entity =>
			{
				entity.ToTable("MBRelease");

				entity.HasIndex(e => e.ID, "IX_MBRelease_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.BoxUrl).HasColumnName("BoxURL");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.MBGame_ID).HasColumnName("MBGame_ID");

				entity.Property(e => e.MBPlatform_ID).HasColumnName("MBPlatform_ID");

				entity.Property(e => e.Region_ID)
					.HasColumnName("Region_ID")
					.HasDefaultValueSql("0");

				entity.Property(e => e.ScreenUrl).HasColumnName("ScreenURL");

				entity.Property(e => e.Title).IsRequired();

				entity.HasOne(d => d.MBGame)
					.WithMany(p => p.MBReleases)
					.HasForeignKey(d => d.MBGame_ID).HasAnnotation("Column", "MBGame_ID")
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.MBPlatform)
					.WithMany(p => p.MBReleases)
					.HasForeignKey(d => d.MBPlatform_ID).HasAnnotation("Column", "MBPlatform_ID");

				entity.HasOne(d => d.Region)
					.WithMany(p => p.MBReleases)
					.HasForeignKey(d => d.Region_ID);
			});

			modelBuilder.Entity<OVGPlatform>(entity =>
			{
				entity.ToTable("OVGPlatform");

				entity.HasIndex(e => e.ID, "IX_OVGPlatform_ID")
					.IsUnique();

				entity.HasIndex(e => e.Title, "IX_OVGPlatform_Title")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<OVGRelease>(entity =>
			{
				entity.ToTable("OVGRelease");

				entity.HasIndex(e => e.ID, "IX_OVGRelease_ID")
					.IsUnique();

				entity.Property(e => e.ID).HasColumnName("ID");

				entity.Property(e => e.BoxBackUrl).HasColumnName("BoxBackUrl");

				entity.Property(e => e.BoxFrontUrl).HasColumnName("BoxFrontUrl");

				entity.Property(e => e.Crc).HasColumnName("CRC");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.MD5).HasColumnName("Md5");

				entity.Property(e => e.OVGPlatform_ID).HasColumnName("OVGPlatform_ID");

				entity.Property(e => e.ReferenceImageUrl).HasColumnName("ReferenceImageURL");

				entity.Property(e => e.ReferenceUrl).HasColumnName("ReferenceURL");

				entity.Property(e => e.Region_ID)
					.HasColumnName("Region_ID")
					.HasDefaultValueSql("0");

				entity.Property(e => e.SHA1).HasColumnName("Sha1");

				entity.HasOne(d => d.OVGPlatform)
					.WithMany(p => p.OVGReleases)
					.HasForeignKey(d => d.OVGPlatform_ID);

				entity.HasOne(d => d.Region)
					.WithMany(p => p.OVGReleases)
					.HasForeignKey(d => d.Region_ID);
			});

			modelBuilder.Entity<Platform>(entity =>
			{
				entity.ToTable("Platform");

				entity.HasIndex(e => e.Abbreviation, "IX_Platform_Abbreviation")
					.IsUnique();

				entity.HasIndex(e => e.FileName, "IX_Platform_FileName")
					.IsUnique();

				entity.HasIndex(e => e.HiganRomFolder, "IX_Platform_HiganRomFolder")
					.IsUnique();

				entity.HasIndex(e => e.ID, "IX_Platform_ID")
					.IsUnique();

				entity.HasIndex(e => e.ID_GB, "IX_PlatformID_GB")
					.IsUnique();

				entity.HasIndex(e => e.ID_GDB, "IX_PlatformID_GDB")
					.IsUnique();

				entity.HasIndex(e => e.ID_LB, "IX_PlatformID_LB")
				.IsUnique();

				//entity.HasIndex(e => e.ID_MB, "IX_PlatformID_MB")
				//.IsUnique();

				entity.HasIndex(e => e.Title, "IX_Platform_Title")
					.IsUnique();

				entity.HasIndex(e => e.ID, "PID");

				entity.Property(e => e.ID).HasColumnName("ID");

				entity.Property(e => e.CacheDate)
					.IsRequired()
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("1900 - 1 - 1 - 0 - 0 - 0");

				entity.Property(e => e.Date)
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("0");

				entity.Property(e => e.FileName).IsRequired();

				entity.Property(e => e.HiganExtension).HasColumnType("STRING");

				entity.Property(e => e.ID_GB).HasColumnName("ID_GB");

				entity.Property(e => e.ID_GDB).HasColumnName("ID_GDB");

				entity.Property(e => e.ID_LB).HasColumnName("ID_LB");

				entity.Property(e => e.ID_MB).HasColumnName("ID_MB");

				entity.Property(e => e.LastDate)
					.IsRequired()
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("1900 - 1 - 1 - 0 - 0 - 0");

				entity.Property(e => e.Preferred)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");

				entity.Property(e => e.PreferredEmulator_ID)
					.HasColumnName("PreferredEmulator_ID")
					.HasDefaultValueSql("7");

				entity.Property(e => e.Rating)
					.IsRequired()
					.HasColumnType("NUMERIC")
					.HasDefaultValueSql("0");

				entity.HasOne(d => d.GBPlatform)
					.WithOne(p => p.Platform)
					.HasForeignKey<Platform>(d => d.ID_GB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.GDBPlatform)
					.WithOne(p => p.Platform)
					.HasForeignKey<Platform>(d => d.ID_GDB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.MBPlatform)
					.WithOne(p => p.Platform)
					.HasForeignKey<Platform>(d => d.ID_MB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.LBPlatform)
					.WithOne(p => p.Platform)
					.HasForeignKey<Platform>(d => d.ID_LB)
					.OnDelete(DeleteBehavior.SetNull);



				entity.HasOne(d => d.PreferredEmulator)
					.WithMany(p => p.Platforms1)
					.HasForeignKey(d => d.PreferredEmulator_ID)
					.OnDelete(DeleteBehavior.ClientSetNull);

				entity.HasMany(d => d.Emulators)
					.WithMany(p => p.Platforms)
					.UsingEntity<Dictionary<string, object>>(
						"PlatformEmulator",
						l => l.HasOne<Emulator>().WithMany().HasForeignKey("Emulator_ID"),
						r => r.HasOne<Platform>().WithMany().HasForeignKey("Platform_ID"),
						j =>
						{
							j.HasKey("Platform_ID", "Emulator_ID");

							j.ToTable("Platform_Emulator");

							j.HasIndex(new[] { "Emulator_ID", "Platform_ID" }, "PE");

							j.IndexerProperty<long>("PlatformId").HasColumnName("Platform_ID");

							j.IndexerProperty<long>("EmulatorId").HasColumnName("Emulator_ID");
						});
			});

			modelBuilder.Entity<Region>(entity =>
			{
				entity.ToTable("Region");

				entity.HasIndex(e => e.Title, "IX_Region_Title")
					.IsUnique();

				entity.HasIndex(e => e.UNCode, "IX_Region_UNCode")
					.IsUnique();

				entity.Property(e => e.ID)
					.HasColumnType("integer")
					.HasColumnName("ID");

				entity.Property(e => e.Datomatic).HasColumnType("STRING");

				entity.Property(e => e.ID_GB).HasColumnName("ID_GB");

				entity.Property(e => e.Priority).HasDefaultValueSql("0");

				entity.Property(e => e.Title)
					.IsRequired()
					.HasColumnType("text");

				entity.Property(e => e.TitleGb).HasColumnName("Title_GB");

				entity.Property(e => e.UNCode).HasColumnName("UNCode");
			});

			modelBuilder.Entity<Release>(entity =>
			{
				entity.ToTable("Release");

				entity.HasIndex(e => e.ID, "IX_Release_ID")
					.IsUnique();

				entity.HasIndex(e => e.ID, "RID");

				entity.Property(e => e.ID).HasColumnName("ID");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.Game_ID).HasColumnName("Game_ID");

				entity.Property(e => e.ID_GB).HasColumnName("ID_GB");

				entity.Property(e => e.ID_GDB).HasColumnName("ID_GDB");

				entity.Property(e => e.ID_LB).HasColumnName("ID_LB");

				entity.Property(e => e.ID_LBG).HasColumnName("ID_LBG");

				entity.Property(e => e.ID_MB).HasColumnName("ID_MB");

				entity.Property(e => e.ID_OVG).HasColumnName("ID_OVG");

				entity.Property(e => e.Platform_ID).HasColumnName("Platform_ID");

				entity.Property(e => e.Preferred)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");

				entity.Property(e => e.Region_ID).HasColumnName("Region_ID");

				entity.Property(e => e.Rom_ID).HasColumnName("Rom_ID");

				entity.Property(e => e.Title).IsRequired();

				entity.HasOne(d => d.Game)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.Game_ID)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(d => d.GBRelease)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_GB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.MBRelease)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_MB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.GDBRelease)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_GDB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.LBRelease)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_LB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.LBGame)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_LBG)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.OVGRelease)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_OVG)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Platform)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.Platform_ID);

				entity.HasOne(d => d.Region)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.Region_ID)
					.OnDelete(DeleteBehavior.ClientSetNull);

				entity.HasOne(d => d.Rom)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.Rom_ID)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Rom>(entity =>
			{
				entity.ToTable("Rom");

				entity.HasIndex(e => e.CRC32, "IX_Rom_Crc32")
					.IsUnique();

				entity.HasIndex(e => e.FileName, "IX_Rom_FileName")
					.IsUnique();

				entity.HasIndex(e => e.ID, "IX_RomId")
					.IsUnique();

				entity.HasIndex(e => e.MD5, "IX_Rom_Md5")
					.IsUnique();

				entity.HasIndex(e => e.SHA1, "IX_Rom_Sha1")
					.IsUnique();

				entity.Property(e => e.ID).HasColumnName("ID");

				entity.Property(e => e.CRC32).HasColumnName("Crc32");

				entity.Property(e => e.MD5).HasColumnName("Md5");

				entity.Property(e => e.SHA1).HasColumnName("Sha1");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<URL>(entity =>
			{
				entity.ToTable("URL");

				entity.HasIndex(e => e.ID, "IX_URL_ID")
					.IsUnique();

				entity.Property(e => e.ID)
					.ValueGeneratedNever()
					.HasColumnName("ID");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}
