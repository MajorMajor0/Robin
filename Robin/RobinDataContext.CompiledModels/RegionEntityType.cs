﻿// <auto-generated />
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Robin
{
    internal partial class RegionEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Robin.Region",
                typeof(Region),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(long),
                propertyInfo: typeof(Region).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw);
            id.AddAnnotation("Relational:ColumnName", "ID");
            id.AddAnnotation("Relational:ColumnType", "integer");

            var datomatic = runtimeEntityType.AddProperty(
                "Datomatic",
                typeof(string),
                propertyInfo: typeof(Region).GetProperty("Datomatic", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<Datomatic>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            datomatic.AddAnnotation("Relational:ColumnType", "STRING");

            var idGb = runtimeEntityType.AddProperty(
                "IdGb",
                typeof(long?),
                propertyInfo: typeof(Region).GetProperty("IdGb", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<IdGb>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            idGb.AddAnnotation("Relational:ColumnName", "ID_GB");

            var launchbox = runtimeEntityType.AddProperty(
                "Launchbox",
                typeof(string),
                propertyInfo: typeof(Region).GetProperty("Launchbox", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<Launchbox>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var mame = runtimeEntityType.AddProperty(
                "Mame",
                typeof(string),
                propertyInfo: typeof(Region).GetProperty("Mame", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<Mame>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);

            var priority = runtimeEntityType.AddProperty(
                "Priority",
                typeof(long?),
                propertyInfo: typeof(Region).GetProperty("Priority", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<Priority>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true,
                valueGenerated: ValueGenerated.OnAdd);
            priority.AddAnnotation("Relational:DefaultValueSql", "0");

            var title = runtimeEntityType.AddProperty(
                "Title",
                typeof(string),
                propertyInfo: typeof(Region).GetProperty("Title", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<Title>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            title.AddAnnotation("Relational:ColumnType", "text");

            var titleGb = runtimeEntityType.AddProperty(
                "TitleGb",
                typeof(string),
                propertyInfo: typeof(Region).GetProperty("TitleGb", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<TitleGb>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            titleGb.AddAnnotation("Relational:ColumnName", "Title_GB");

            var uncode = runtimeEntityType.AddProperty(
                "Uncode",
                typeof(string),
                propertyInfo: typeof(Region).GetProperty("Uncode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Region).GetField("<Uncode>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                nullable: true);
            uncode.AddAnnotation("Relational:ColumnName", "UNCode");

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var iX_Region_Title = runtimeEntityType.AddIndex(
                new[] { title },
                name: "IX_Region_Title",
                unique: true);

            var iX_Region_UNCode = runtimeEntityType.AddIndex(
                new[] { uncode },
                name: "IX_Region_UNCode",
                unique: true);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Region");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}
