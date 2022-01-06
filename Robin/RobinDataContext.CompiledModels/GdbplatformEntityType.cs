﻿// <auto-generated />
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Robin
{
    internal partial class GdbplatformEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Robin.Gdbplatform",
                typeof(Gdbplatform),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(long),
                propertyInfo: typeof(Gdbplatform).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                afterSaveBehavior: PropertySaveBehavior.Throw);
            id.AddAnnotation("Relational:ColumnName", "ID");

            var bannerUrl = runtimeEntityType.AddProperty(
                "BannerUrl",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("BannerUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<BannerUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            bannerUrl.AddAnnotation("Relational:ColumnName", "BannerUrl");

            var boxBackUrl = runtimeEntityType.AddProperty(
                "BoxBackUrl",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("BoxBackUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<BoxBackUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            boxBackUrl.AddAnnotation("Relational:ColumnName", "BoxBackUrl");

            var boxFrontUrl = runtimeEntityType.AddProperty(
                "BoxFrontUrl",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("BoxFrontUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<BoxFrontUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            boxFrontUrl.AddAnnotation("Relational:ColumnName", "BoxFrontUrl");

            var cacheDate = runtimeEntityType.AddProperty(
                "CacheDate",
                typeof(DateTime),
                propertyInfo: typeof(Gdbplatform).GetProperty("CacheDate", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<CacheDate>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd);
            cacheDate.AddAnnotation("Relational:ColumnType", "DATETIME");
            cacheDate.AddAnnotation("Relational:DefaultValueSql", "1900 - 1 - 1 - 0 - 0 - 0");

            var consoleUrl = runtimeEntityType.AddProperty(
                "ConsoleUrl",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("ConsoleUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<ConsoleUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            consoleUrl.AddAnnotation("Relational:ColumnName", "ConsoleUrl");

            var controllerUrl = runtimeEntityType.AddProperty(
                "ControllerUrl",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("ControllerUrl", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<ControllerUrl>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            controllerUrl.AddAnnotation("Relational:ColumnName", "ControllerUrl");

            var controllers = runtimeEntityType.AddProperty(
                "Controllers",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("Controllers", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Controllers>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var cpu = runtimeEntityType.AddProperty(
                "Cpu",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("Cpu", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Cpu>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var date = runtimeEntityType.AddProperty(
                "Date",
                typeof(DateTime?),
                propertyInfo: typeof(Gdbplatform).GetProperty("Date", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Date>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            date.AddAnnotation("Relational:ColumnType", "DATETIME");

            var developer = runtimeEntityType.AddProperty(
                "Developer",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("Developer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Developer>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var display = runtimeEntityType.AddProperty(
                "Display",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("Display", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Display>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var manufacturer = runtimeEntityType.AddProperty(
                "Manufacturer",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("Manufacturer", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Manufacturer>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var media = runtimeEntityType.AddProperty(
                "Media",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("Media", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Media>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var overview = runtimeEntityType.AddProperty(
                "Overview",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("Overview", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Overview>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var rating = runtimeEntityType.AddProperty(
                "Rating",
                typeof(double?),
                propertyInfo: typeof(Gdbplatform).GetProperty("Rating", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Rating>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            rating.AddAnnotation("Relational:ColumnType", "NUMERIC");

            var sound = runtimeEntityType.AddProperty(
                "Sound",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("Sound", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Sound>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var title = runtimeEntityType.AddProperty(
                "Title",
                typeof(string),
                propertyInfo: typeof(Gdbplatform).GetProperty("Title", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Gdbplatform).GetField("<Title>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var iX_GdbplatformId = runtimeEntityType.AddIndex(
                new[] { id },
                name: "IX_GdbplatformId",
                unique: true);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Gdbplatform");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
