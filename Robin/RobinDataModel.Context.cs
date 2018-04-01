﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Robin
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RobinDataEntities : Entity
    {
        public RobinDataEntities()
            : base("name=RobinDataEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Collection> Collections { get; set; }
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
        public virtual DbSet<OVGPlatform> OVGPlatforms { get; set; }
        public virtual DbSet<OVGRelease> OVGReleases { get; set; }
        public virtual DbSet<Platform> Platforms { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Release> Releases { get; set; }
        public virtual DbSet<Rom> Roms { get; set; }
        public virtual DbSet<URL> URLs { get; set; }
    }
}
