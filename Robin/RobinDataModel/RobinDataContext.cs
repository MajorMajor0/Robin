using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Robin
{
	public partial class RobinDataEntities : Entity
	{
		public RobinDataEntities()
		{
		}

		//public RobinDataEntities(DbContextOptions<RobinDataEntities> options)
		//    : base(options)
		//{
		//}

		public virtual DbSet<Collection> Collections { get; set; }
		public virtual DbSet<Emulator> Emulators { get; set; }
		public virtual DbSet<Game> Games { get; set; }
		public virtual DbSet<Gbgame> Gbgames { get; set; }
		public virtual DbSet<Gbplatform> Gbplatforms { get; set; }
		public virtual DbSet<Gbrelease> Gbreleases { get; set; }
		public virtual DbSet<Gdbplatform> Gdbplatforms { get; set; }
		public virtual DbSet<Gdbrelease> Gdbreleases { get; set; }
		public virtual DbSet<Language> Languages { get; set; }
		public virtual DbSet<Lbgame> Lbgames { get; set; }
		public virtual DbSet<Lbimage> Lbimages { get; set; }
		public virtual DbSet<Lbplatform> Lbplatforms { get; set; }
		public virtual DbSet<Lbrelease> Lbreleases { get; set; }
		public virtual DbSet<Match> Matches { get; set; }
		public virtual DbSet<Ovgplatform> Ovgplatforms { get; set; }
		public virtual DbSet<Ovgrelease> Ovgreleases { get; set; }
		public virtual DbSet<Platform> Platforms { get; set; }
		public virtual DbSet<Region> Regions { get; set; }
		public virtual DbSet<Release> Releases { get; set; }
		public virtual DbSet<Rom> Roms { get; set; }
		public virtual DbSet<Url> Urls { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
				optionsBuilder.UseSqlite($"DataSource={FileLocation}");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Collection>(entity =>
			{
				entity.ToTable("Collection");

				entity.HasIndex(e => e.Id, "IX_Collection_ID")
					.IsUnique();

				entity.Property(e => e.Id).HasColumnName("ID");

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

			modelBuilder.Entity<Emulator>(entity =>
			{
				entity.ToTable("Emulator");

				entity.HasIndex(e => e.Id, "IX_Emulator_ID")
					.IsUnique();

				entity.Property(e => e.Id)
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

				entity.HasIndex(e => e.Id, "IX_Game_ID")
					.IsUnique();

				entity.HasIndex(e => e.Id, "GID");

				entity.Property(e => e.Id).HasColumnName("ID");

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

			modelBuilder.Entity<Gbgame>(entity =>
			{
				entity.ToTable("Gbgame");

				entity.Property(e => e.Id)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.BoxFrontUrl).HasColumnName("BoxFrontUrl");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.GbplatformId).HasColumnName("GbplatformId");

				entity.Property(e => e.ScreenUrl).HasColumnName("ScreenUrl");

				entity.HasOne(d => d.Gbplatform)
					.WithMany(p => p.Gbgames)
					.HasForeignKey(d => d.GbplatformId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Gbplatform>(entity =>
			{
				entity.ToTable("Gbplatform");

				entity.HasIndex(e => e.Id, "IX_GbplatformId")
					.IsUnique();

				entity.Property(e => e.Id)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.CacheDate)
					.IsRequired()
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("1900 - 1 - 1 - 0 - 0 - 0");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<Gbrelease>(entity =>
			{
				entity.ToTable("Gbrelease");

				entity.HasIndex(e => e.Id, "IX_Gbrelease_ID")
					.IsUnique();

				entity.Property(e => e.Id)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.BoxUrl).HasColumnName("BoxUrl");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.GbgameId).HasColumnName("GbgameId");

				entity.Property(e => e.GbplatformId).HasColumnName("GbplatformId");

				entity.Property(e => e.RegionId)
					.HasColumnName("RegionId")
					.HasDefaultValueSql("0");

				entity.Property(e => e.ScreenUrl).HasColumnName("ScreenUrl");

				entity.Property(e => e.Title).IsRequired();

				entity.HasOne(d => d.Gbgame)
					.WithMany(p => p.Gbreleases)
					.HasForeignKey(d => d.GbgameId)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Gbplatform)
					.WithMany(p => p.Gbreleases)
					.HasForeignKey(d => d.GbplatformId);

				entity.HasOne(d => d.Region)
					.WithMany(p => p.Gbreleases)
					.HasForeignKey(d => d.RegionId);
			});

			modelBuilder.Entity<Gdbplatform>(entity =>
			{
				entity.ToTable("Gdbplatform");

				entity.HasIndex(e => e.Id, "IX_GdbplatformId")
					.IsUnique();

				entity.Property(e => e.Id)
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

			modelBuilder.Entity<Gdbrelease>(entity =>
			{
				entity.ToTable("Gdbrelease");

				entity.HasIndex(e => e.Id, "IX_Gdbrelease_ID")
					.IsUnique();

				entity.Property(e => e.Id)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.BannerUrl).HasColumnName("BannerUrl");

				entity.Property(e => e.BoxBackUrl).HasColumnName("BoxBackUrl");

				entity.Property(e => e.BoxFrontUrl).HasColumnName("BoxFrontUrl");

				entity.Property(e => e.Coop).HasColumnType("BOOLEAN");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.GdbplatformId).HasColumnName("GdbplatformId");

				entity.Property(e => e.LogoUrl).HasColumnName("LogoUrl");

				entity.Property(e => e.Rating).HasColumnType("NUMERIC");

				entity.Property(e => e.ScreenUrl).HasColumnName("ScreenUrl");

				entity.HasOne(d => d.Gdbplatform)
					.WithMany(p => p.Gdbreleases)
					.HasForeignKey(d => d.GdbplatformId);
			});

			modelBuilder.Entity<Language>(entity =>
			{
				entity.ToTable("Language");

				entity.HasIndex(e => e.Abbreviation, "IX_Language_Abbreviation")
					.IsUnique();

				entity.HasIndex(e => e.Title, "IX_Language_Title")
					.IsUnique();

				entity.Property(e => e.Id)
					.HasColumnType("integer")
					.HasColumnName("ID");

				entity.Property(e => e.Abbreviation)
					.IsRequired()
					.HasColumnType("text");

				entity.Property(e => e.Title)
					.IsRequired()
					.HasColumnType("text");
			});

			modelBuilder.Entity<Lbgame>(entity =>
			{
				entity.ToTable("Lbgame");

				entity.HasIndex(e => e.Id, "IX_Lbgame_ID")
					.IsUnique();

				entity.Property(e => e.Id)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.LbplatformId).HasColumnName("LbplatformId");

				entity.Property(e => e.Title).IsRequired();

				entity.Property(e => e.VideoUrl).HasColumnName("VideoUrl");

				entity.Property(e => e.WikiUrl).HasColumnName("WikiUrl");

				entity.HasOne(d => d.Lbplatform)
					.WithMany(p => p.Lbgames)
					.HasForeignKey(d => d.LbplatformId);
			});

			modelBuilder.Entity<Lbimage>(entity =>
			{
				entity.ToTable("Lbimages");

				entity.HasIndex(e => e.FileName, "IX_Lbimages_FileName")
					.IsUnique();

				entity.HasIndex(e => e.Id, "IX_Lbimages_ID")
					.IsUnique();

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.FileName)
					.IsRequired()
					.HasColumnType("STRING");

				entity.Property(e => e.Lbregion).HasColumnName("LBRegion");

				entity.Property(e => e.LbreleaseId).HasColumnName("Lbrelease_ID");

				entity.Property(e => e.RegionId).HasColumnName("RegionId");

				entity.Property(e => e.Type).IsRequired();

				entity.HasOne(d => d.Lbrelease)
					.WithMany(p => p.Lbimages)
					.HasForeignKey(d => d.LbreleaseId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Lbplatform>(entity =>
			{
				entity.ToTable("Lbplatform");

				entity.HasIndex(e => e.Id, "IX_LbplatformId")
					.IsUnique();

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.CacheDate)
					.IsRequired()
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("1900 - 1 - 1 - 0 - 0 - 0");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<Lbrelease>(entity =>
			{
				entity.ToTable("Lbrelease");

				entity.HasIndex(e => e.Id, "IX_Lbrelease_ID")
					.IsUnique();

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.LbgameId).HasColumnName("Lbgame_ID");

				entity.Property(e => e.RegionId).HasColumnName("RegionId");

				entity.HasOne(d => d.Lbgame)
					.WithMany(p => p.Lbreleases)
					.HasForeignKey(d => d.LbgameId);

				entity.HasOne(d => d.Region)
					.WithMany(p => p.Lbreleases)
					.HasForeignKey(d => d.RegionId)
					.OnDelete(DeleteBehavior.ClientSetNull);
			});

			modelBuilder.Entity<Match>(entity =>
			{
				entity.ToTable("Match");

				entity.HasIndex(e => e.Id, "IX_Match_ID")
					.IsUnique();

				entity.Property(e => e.Id)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.IdGb).HasColumnName("ID_GB");

				entity.Property(e => e.IdGdb).HasColumnName("ID_GDB");

				entity.Property(e => e.IdOvg).HasColumnName("ID_OVG");

				entity.Property(e => e.RegionId)
					.HasColumnName("RegionId")
					.HasDefaultValueSql("0");

				entity.Property(e => e.Sha1)
					.IsRequired()
					.HasColumnName("Sha1");
			});

			modelBuilder.Entity<Ovgplatform>(entity =>
			{
				entity.ToTable("Ovgplatform");

				entity.HasIndex(e => e.Id, "IX_OvgPlatformId")
					.IsUnique();

				entity.HasIndex(e => e.Title, "IX_Ovgplatform_Title")
					.IsUnique();

				entity.Property(e => e.Id)
					.ValueGeneratedNever()
					.HasColumnName("ID");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<Ovgrelease>(entity =>
			{
				entity.ToTable("Ovgrelease");

				entity.HasIndex(e => e.Id, "IX_Ovgrelease_ID")
					.IsUnique();

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.BoxBackUrl).HasColumnName("BoxBackUrl");

				entity.Property(e => e.BoxFrontUrl).HasColumnName("BoxFrontUrl");

				entity.Property(e => e.Crc).HasColumnName("CRC");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.Md5).HasColumnName("Md5");

				entity.Property(e => e.OvgplatformId).HasColumnName("OvgPlatformId");

				entity.Property(e => e.ReferenceImageUrl).HasColumnName("ReferenceImageURL");

				entity.Property(e => e.ReferenceUrl).HasColumnName("ReferenceURL");

				entity.Property(e => e.RegionId)
					.HasColumnName("RegionId")
					.HasDefaultValueSql("0");

				entity.Property(e => e.Sha1).HasColumnName("Sha1");

				entity.HasOne(d => d.Ovgplatform)
					.WithMany(p => p.Ovgreleases)
					.HasForeignKey(d => d.OvgplatformId);

				entity.HasOne(d => d.Region)
					.WithMany(p => p.Ovgreleases)
					.HasForeignKey(d => d.RegionId);
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

				entity.HasIndex(e => e.Id, "IX_PlatformId")
					.IsUnique();

				entity.HasIndex(e => e.ID_GB, "IX_PlatformID_GB")
					.IsUnique();

				entity.HasIndex(e => e.ID_GDB, "IX_PlatformID_GDB")
					.IsUnique();

				entity.HasIndex(e => e.ID_LB, "IX_PlatformID_LB")
					.IsUnique();

				entity.HasIndex(e => e.Title, "IX_Platform_Title")
					.IsUnique();

				entity.HasIndex(e => e.Id, "PID");

				entity.Property(e => e.Id).HasColumnName("ID");

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

				entity.Property(e => e.LastDate)
					.IsRequired()
					.HasColumnType("DATETIME")
					.HasDefaultValueSql("1900 - 1 - 1 - 0 - 0 - 0");

				entity.Property(e => e.Preferred)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");

				entity.Property(e => e.PreferredEmulatorId)
					.HasColumnName("PreferredEmulator_ID")
					.HasDefaultValueSql("7");

				entity.Property(e => e.Rating)
					.IsRequired()
					.HasColumnType("NUMERIC")
					.HasDefaultValueSql("0");

				entity.HasOne(d => d.Gbplatform)
					.WithOne(p => p.Platform)
					.HasForeignKey<Platform>(d => d.ID_GB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Gdbplatform)
					.WithOne(p => p.Platform)
					.HasForeignKey<Platform>(d => d.ID_GDB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Lbplatform)
					.WithOne(p => p.Platform)
					.HasForeignKey<Platform>(d => d.ID_LB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.PreferredEmulator)
					.WithMany(p => p.Platforms1)
					.HasForeignKey(d => d.PreferredEmulatorId)
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

				entity.HasIndex(e => e.Uncode, "IX_Region_UNCode")
					.IsUnique();

				entity.Property(e => e.Id)
					.HasColumnType("integer")
					.HasColumnName("ID");

				entity.Property(e => e.Datomatic).HasColumnType("STRING");

				entity.Property(e => e.IdGb).HasColumnName("ID_GB");

				entity.Property(e => e.Priority).HasDefaultValueSql("0");

				entity.Property(e => e.Title)
					.IsRequired()
					.HasColumnType("text");

				entity.Property(e => e.TitleGb).HasColumnName("Title_GB");

				entity.Property(e => e.Uncode).HasColumnName("UNCode");
			});

			modelBuilder.Entity<Release>(entity =>
			{
				entity.ToTable("Release");

				entity.HasIndex(e => e.Id, "IX_Release_ID")
					.IsUnique();

				entity.HasIndex(e => e.Id, "RID");

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Date).HasColumnType("DATETIME");

				entity.Property(e => e.GameId).HasColumnName("Game_ID");

				entity.Property(e => e.ID_GB).HasColumnName("ID_GB");

				entity.Property(e => e.ID_GDB).HasColumnName("ID_GDB");

				entity.Property(e => e.ID_LB).HasColumnName("ID_LB");

				entity.Property(e => e.ID_LBG).HasColumnName("ID_LBG");

				entity.Property(e => e.ID_OVG).HasColumnName("ID_OVG");

				entity.Property(e => e.PlatformId).HasColumnName("Platform_ID");

				entity.Property(e => e.Preferred)
					.IsRequired()
					.HasColumnType("bit")
					.HasDefaultValueSql("0");

				entity.Property(e => e.RegionId).HasColumnName("Region_ID");

				entity.Property(e => e.RomId).HasColumnName("Rom_ID");

				entity.Property(e => e.Title).IsRequired();

				entity.HasOne(d => d.Game)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.GameId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(d => d.Gbrelease)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_GB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Gdbrelease)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_GDB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Lbrelease)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_LB)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Lbgame)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_LBG)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Ovgrelease)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.ID_OVG)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Platform)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.PlatformId);

				entity.HasOne(d => d.Region)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.RegionId)
					.OnDelete(DeleteBehavior.ClientSetNull);

				entity.HasOne(d => d.Rom)
					.WithMany(p => p.Releases)
					.HasForeignKey(d => d.RomId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Rom>(entity =>
			{
				entity.ToTable("Rom");

				entity.HasIndex(e => e.Crc32, "IX_Rom_Crc32")
					.IsUnique();

				entity.HasIndex(e => e.FileName, "IX_Rom_FileName")
					.IsUnique();

				entity.HasIndex(e => e.Id, "IX_RomId")
					.IsUnique();

				entity.HasIndex(e => e.Md5, "IX_Rom_Md5")
					.IsUnique();

				entity.HasIndex(e => e.Sha1, "IX_Rom_Sha1")
					.IsUnique();

				entity.Property(e => e.Id).HasColumnName("ID");

				entity.Property(e => e.Crc32).HasColumnName("Crc32");

				entity.Property(e => e.Md5).HasColumnName("Md5");

				entity.Property(e => e.Sha1).HasColumnName("Sha1");

				entity.Property(e => e.Title).IsRequired();
			});

			modelBuilder.Entity<Url>(entity =>
			{
				entity.ToTable("URL");

				entity.HasIndex(e => e.Id, "IX_URL_ID")
					.IsUnique();

				entity.Property(e => e.Id)
					.ValueGeneratedNever()
					.HasColumnName("ID");
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}
