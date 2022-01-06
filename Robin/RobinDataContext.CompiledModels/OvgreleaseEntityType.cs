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
    internal partial class OvgreleaseEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Robin.Ovgrelease",
                typeof(Ovgrelease),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(long),
                propertyInfo: typeof(Ovgrelease).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw);
            id.AddAnnotation("Relational:ColumnName", "ID");

            var boxBackUrl = runtimeEntityType.AddProperty(
                "BoxBackUrl",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("BoxBackUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<BoxBackUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            boxBackUrl.AddAnnotation("Relational:ColumnName", "BoxBackUrl");

            var boxFrontUrl = runtimeEntityType.AddProperty(
                "BoxFrontUrl",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("BoxFrontUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<BoxFrontUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            boxFrontUrl.AddAnnotation("Relational:ColumnName", "BoxFrontUrl");

            var crc = runtimeEntityType.AddProperty(
                "Crc",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Crc", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Crc>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            crc.AddAnnotation("Relational:ColumnName", "CRC");

            var date = runtimeEntityType.AddProperty(
                "Date",
                typeof(DateTime?),
                propertyInfo: typeof(Ovgrelease).GetProperty("Date", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Date>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            date.AddAnnotation("Relational:ColumnType", "DATETIME");

            var dateText = runtimeEntityType.AddProperty(
                "DateText",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("DateText", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<DateText>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var developer = runtimeEntityType.AddProperty(
                "Developer",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Developer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Developer>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var genre = runtimeEntityType.AddProperty(
                "Genre",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Genre", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Genre>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var header = runtimeEntityType.AddProperty(
                "Header",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Header", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Header>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var language = runtimeEntityType.AddProperty(
                "Language",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Language", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Language>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var md5 = runtimeEntityType.AddProperty(
                "Md5",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Md5", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Md5>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            md5.AddAnnotation("Relational:ColumnName", "Md5");

            var overview = runtimeEntityType.AddProperty(
                "Overview",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Overview", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Overview>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var ovgplatformId = runtimeEntityType.AddProperty(
                "OvgplatformId",
                typeof(long),
                propertyInfo: typeof(Ovgrelease).GetProperty("OvgplatformId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<OvgplatformId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            ovgplatformId.AddAnnotation("Relational:ColumnName", "OvgPlatformId");

            var publisher = runtimeEntityType.AddProperty(
                "Publisher",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Publisher", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Publisher>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var referenceImageUrl = runtimeEntityType.AddProperty(
                "ReferenceImageUrl",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("ReferenceImageUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<ReferenceImageUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            referenceImageUrl.AddAnnotation("Relational:ColumnName", "ReferenceImageURL");

            var referenceUrl = runtimeEntityType.AddProperty(
                "ReferenceUrl",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("ReferenceUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<ReferenceUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            referenceUrl.AddAnnotation("Relational:ColumnName", "ReferenceURL");

            var regionId = runtimeEntityType.AddProperty(
                "RegionId",
                typeof(long?),
                propertyInfo: typeof(Ovgrelease).GetProperty("RegionId", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<RegionId>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                valueGenerated: ValueGenerated.OnAdd);
            regionId.AddAnnotation("Relational:ColumnName", "RegionId");
            regionId.AddAnnotation("Relational:DefaultValueSql", "0");

            var serial = runtimeEntityType.AddProperty(
                "Serial",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Serial", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Serial>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var sha1 = runtimeEntityType.AddProperty(
                "Sha1",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Sha1", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Sha1>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            sha1.AddAnnotation("Relational:ColumnName", "Sha1");

            var size = runtimeEntityType.AddProperty(
                "Size",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Size", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Size>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var title = runtimeEntityType.AddProperty(
                "Title",
                typeof(string),
                propertyInfo: typeof(Ovgrelease).GetProperty("Title", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Title>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var index = runtimeEntityType.AddIndex(
                new[] { ovgplatformId });

            var index0 = runtimeEntityType.AddIndex(
                new[] { regionId });

            var iX_Ovgrelease_ID = runtimeEntityType.AddIndex(
                new[] { id },
                name: "IX_Ovgrelease_ID",
                unique: true);

            return runtimeEntityType;
        }

        public static RuntimeForeignKey CreateForeignKey1(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("OvgplatformId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType,
                deleteBehavior: DeleteBehavior.Cascade,
                required: true);

            var ovgplatform = declaringEntityType.AddNavigation("Ovgplatform",
                runtimeForeignKey,
                onDependent: true,
                typeof(Ovgplatform),
                propertyInfo: typeof(Ovgrelease).GetProperty("Ovgplatform", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Ovgplatform>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var ovgreleases = principalEntityType.AddNavigation("Ovgreleases",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Ovgrelease>),
                propertyInfo: typeof(Ovgplatform).GetProperty("Ovgreleases", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgplatform).GetField("<Ovgreleases>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static RuntimeForeignKey CreateForeignKey2(RuntimeEntityType declaringEntityType, RuntimeEntityType principalEntityType)
        {
            var runtimeForeignKey = declaringEntityType.AddForeignKey(new[] { declaringEntityType.FindProperty("RegionId") },
                principalEntityType.FindKey(new[] { principalEntityType.FindProperty("Id") }),
                principalEntityType);

            var region = declaringEntityType.AddNavigation("Region",
                runtimeForeignKey,
                onDependent: true,
                typeof(Region),
                propertyInfo: typeof(Ovgrelease).GetProperty("Region", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Ovgrelease).GetField("<Region>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var ovgreleases = principalEntityType.AddNavigation("Ovgreleases",
                runtimeForeignKey,
                onDependent: false,
                typeof(ICollection<Ovgrelease>),
                propertyInfo: typeof(Region).GetProperty("Ovgreleases", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<Ovgreleases>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            return runtimeForeignKey;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Ovgrelease");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
