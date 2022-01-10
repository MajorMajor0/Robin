﻿// <auto-generated />
using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace Robin
{
    internal partial class LanguageEntityType
    {
        public static RuntimeEntityType Create(RuntimeModel model, RuntimeEntityType baseEntityType = null)
        {
            var runtimeEntityType = model.AddEntityType(
                "Robin.Language",
                typeof(Language),
                baseEntityType);

            var id = runtimeEntityType.AddProperty(
                "Id",
                typeof(long),
                propertyInfo: typeof(Language).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Language).GetField("<Id>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                valueGenerated: ValueGenerated.OnAdd,
                afterSaveBehavior: PropertySaveBehavior.Throw);
            id.AddAnnotation("Relational:ColumnName", "ID");
            id.AddAnnotation("Relational:ColumnType", "integer");

            var abbreviation = runtimeEntityType.AddProperty(
                "Abbreviation",
                typeof(string),
                propertyInfo: typeof(Language).GetProperty("Abbreviation", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Language).GetField("<Abbreviation>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            abbreviation.AddAnnotation("Relational:ColumnType", "text");

            var title = runtimeEntityType.AddProperty(
                "Title",
                typeof(string),
                propertyInfo: typeof(Language).GetProperty("Title", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly),
                fieldInfo: typeof(Language).GetField("<Title>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            title.AddAnnotation("Relational:ColumnType", "text");

            var key = runtimeEntityType.AddKey(
                new[] { id });
            runtimeEntityType.SetPrimaryKey(key);

            var iX_Language_Abbreviation = runtimeEntityType.AddIndex(
                new[] { abbreviation },
                name: "IX_Language_Abbreviation",
                unique: true);

            var iX_Language_Title = runtimeEntityType.AddIndex(
                new[] { title },
                name: "IX_Language_Title",
                unique: true);

            return runtimeEntityType;
        }

        public static void CreateAnnotations(RuntimeEntityType runtimeEntityType)
        {
            runtimeEntityType.AddAnnotation("Relational:FunctionName", null);
            runtimeEntityType.AddAnnotation("Relational:Schema", null);
            runtimeEntityType.AddAnnotation("Relational:SqlQuery", null);
            runtimeEntityType.AddAnnotation("Relational:TableName", "Language");
            runtimeEntityType.AddAnnotation("Relational:ViewName", null);
            runtimeEntityType.AddAnnotation("Relational:ViewSchema", null);

            Customize(runtimeEntityType);
        }

        static partial void Customize(RuntimeEntityType runtimeEntityType);
    }
}