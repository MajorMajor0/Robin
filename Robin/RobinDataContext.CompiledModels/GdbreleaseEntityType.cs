﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Robin
{
    internal partial class GdbreleaseEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Robin.Gdbrelease",
                typeof(Gdbrelease),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(long),
                propertyInfo: typeof(Gdbrelease).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            id.AddAnnotation("Relational:ColumnName", "ID");

            var bannerUrl = runtimeEntityType.AddProperty(
                "BannerUrl",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("BannerUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<BannerUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            bannerUrl.AddAnnotation("Relational:ColumnName", "BannerUrl");

            var boxBackUrl = runtimeEntityType.AddProperty(
                "BoxBackUrl",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("BoxBackUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<BoxBackUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            boxBackUrl.AddAnnotation("Relational:ColumnName", "BoxBackUrl");

            var boxFrontUrl = runtimeEntityType.AddProperty(
                "BoxFrontUrl",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("BoxFrontUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<BoxFrontUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            boxFrontUrl.AddAnnotation("Relational:ColumnName", "BoxFrontUrl");

            var coop = runtimeEntityType.AddProperty(
                "Coop",
                typeof(bool?),
                propertyInfo: typeof(Gdbrelease).GetProperty("Coop", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Coop>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            coop.AddAnnotation("Relational:ColumnType", "BOOLEAN");

            var date = runtimeEntityType.AddProperty(
                "Date",
                typeof(DateTime?),
                propertyInfo: typeof(Gdbrelease).GetProperty("Date", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Date>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            date.AddAnnotation("Relational:ColumnType", "DATETIME");

            var developer = runtimeEntityType.AddProperty(
                "Developer",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("Developer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Developer>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var gdbplatformId = runtimeEntityType.AddProperty(
                "GdbplatformId",
                typeof(long),
                propertyInfo: typeof(Gdbrelease).GetProperty("GdbplatformId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<GdbplatformId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            gdbplatformId.AddAnnotation("Relational:ColumnName", "GDBPlatform_ID");

            var genre = runtimeEntityType.AddProperty(
                "Genre",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("Genre", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Genre>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var logoUrl = runtimeEntityType.AddProperty(
                "LogoUrl",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("LogoUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<LogoUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            logoUrl.AddAnnotation("Relational:ColumnName", "LogoUrl");

            var overview = runtimeEntityType.AddProperty(
                "Overview",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("Overview", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Overview>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var players = runtimeEntityType.AddProperty(
                "Players",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("Players", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Players>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var publisher = runtimeEntityType.AddProperty(
                "Publisher",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("Publisher", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Publisher>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var rating = runtimeEntityType.AddProperty(
                "Rating",
                typeof(double?),
                propertyInfo: typeof(Gdbrelease).GetProperty("Rating", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Rating>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            rating.AddAnnotation("Relational:ColumnType", "NUMERIC");

            var screenUrl = runtimeEntityType.AddProperty(
                "ScreenUrl",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("ScreenUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<ScreenUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            screenUrl.AddAnnotation("Relational:ColumnName", "ScreenUrl");

            var title = runtimeEntityType.AddProperty(
                "Title",
                typeof(string),
                propertyInfo: typeof(Gdbrelease).GetProperty("Title", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Title>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { gdbplatformId });

            var iX_Gdbrelease_ID = runtimeEntityType.AddIndex(
                new[] { id },
                name: "IX_Gdbrelease_ID",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("GdbplatformId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var gdbplatform = declaringEntityType.AddNavigation("Gdbplatform",
                runtimeForeignKey,
                onDependent: true,
                typeof(Gdbplatform),
                propertyInfo: typeof(Gdbrelease).GetProperty("Gdbplatform", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbrelease).GetField("<Gdbplatform>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var gdbreleases = principalEntityType.AddNavigation("Gdbreleases",
                runtimeForeignKey,
                onDependent: false,
                typeof(HashSet<Gdbrelease>),
                propertyInfo: typeof(Gdbplatform).GetProperty("Gdbreleases", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Gdbreleases>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Gdbrelease");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}