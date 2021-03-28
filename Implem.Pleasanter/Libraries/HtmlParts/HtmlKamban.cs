﻿using Implem.DefinitionAccessor;
using Implem.Libraries.Utilities;
using Implem.Pleasanter.Libraries.Html;
using Implem.Pleasanter.Libraries.Requests;
using Implem.Pleasanter.Libraries.Responses;
using Implem.Pleasanter.Libraries.Settings;
using Implem.Pleasanter.Libraries.ViewModes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
namespace Implem.Pleasanter.Libraries.HtmlParts
{
    public static class HtmlKamban
    {
        public static HtmlBuilder Kamban(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            string aggregateType,
            Column value,
            int columns,
            bool aggregationView,
            IEnumerable<KambanElement> data,
            bool inRange)
        {
            return hb.Div(id: "Kamban", css: "both", action: () =>
            {
                hb
                    .FieldDropDown(
                        context: context,
                        controlId: "KambanGroupByX",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByX(context: context),
                        optionCollection: ss.KambanGroupByOptions(context: context),
                        selectedValue: groupByX.ColumnName,
                        method: "post")
                    .FieldDropDown(
                        context: context,
                        controlId: "KambanGroupByY",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.GroupByY(context: context),
                        optionCollection: ss.KambanGroupByOptions(
                            context: context,
                            addNothing: true),
                        selectedValue: groupByY?.ColumnName,
                        method: "post")
                    .FieldDropDown(
                        context: context,
                        controlId: "KambanAggregateType",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationType(context: context),
                        optionCollection: ss.KambanAggregationTypeOptions(context: context),
                        selectedValue: aggregateType,
                        method: "post")
                    .FieldDropDown(
                        context: context,
                        fieldId: "KambanValueField",
                        controlId: "KambanValue",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationTarget(context: context),
                        optionCollection: ss.KambanValueOptions(context: context),
                        selectedValue: value.ColumnName,
                        method: "post")
                    .FieldDropDown(
                        context: context,
                        controlId: "KambanColumns",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.MaxColumns(context: context),
                        optionCollection: Enumerable.Range(
                            Parameters.General.KambanMinColumns,
                            Parameters.General.KambanMaxColumns)
                                .ToDictionary(o => o.ToString(), o => o.ToString()),
                        selectedValue: columns.ToString(),
                        method: "post")
                    .FieldCheckBox(
                        controlId: "KambanAggregationView",
                        fieldCss: "field-auto-thin",
                        controlCss: " auto-postback",
                        labelText: Displays.AggregationView(context: context),
                        _checked: aggregationView,
                        method: "post")
                    .KambanBody(
                        context: context,
                        ss: ss,
                        view: view,
                        groupByX: groupByX,
                        groupByY: groupByY,
                        aggregateType: aggregateType,
                        value: value,
                        columns: columns,
                        aggregationView: aggregationView,
                        data: data,
                        inRange: inRange);
            });
        }

        public static HtmlBuilder KambanBody(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            View view,
            Column groupByX,
            Column groupByY,
            string aggregateType,
            Column value,
            int columns,
            bool aggregationView,
            IEnumerable<KambanElement> data,
            long changedItemId = 0,
            bool inRange = true)
        {
            if (!inRange) return hb;
            var choicesY = CorrectedChoices(
                groupByY, groupByY?.EditChoices(
                    context: context,
                    insertBlank: true,
                    view: view));
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Id("KambanBody")
                    .DataAction("UpdateByKamban")
                    .DataMethod("post"),
                action: () => groupByX.EditChoices(
                    context: context,
                    insertBlank: true,
                    view: view)
                        .Chunk(columns)
                        .ForEach(choicesX => hb
                            .Table(
                                context: context,
                                ss: ss,
                                choicesX: CorrectedChoices(groupByX, choicesX),
                                choicesY: choicesY,
                                aggregateType: aggregateType,
                                value: value,
                                aggregationView: aggregationView,
                                data: data,
                                changedItemId: changedItemId)));
        }

        private static Dictionary<string, ControlData> CorrectedChoices(
            Column groupBy, IEnumerable<KeyValuePair<string, ControlData>> choices)
        {
            return groupBy != null
                ? groupBy.TypeName.CsTypeSummary() != Types.CsNumeric
                    ? choices.ToDictionary(o => o.Key, o => o.Value)
                    : choices.ToDictionary(
                        o => o.Key != string.Empty ? o.Key : "0",
                        o => o.Value)
                : null;
        }

        private static HtmlBuilder Table(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            Dictionary<string, ControlData> choicesX,
            Dictionary<string, ControlData> choicesY,
            string aggregateType,
            Column value,
            bool aggregationView,
            IEnumerable<KambanElement> data,
            long changedItemId)
        {
            var max = data.Any()
                ? data
                    .GroupBy(o => o.GroupX + "," + o.GroupY)
                    .Select(o => o.Summary(aggregateType))
                    .Max()
                : 0;
            return hb.Table(
                id: "Grid",
                css: "grid fixed",
                action: () => hb
                    .THead(action: () => hb
                        .Tr(css: "ui-widget-header", action: () =>
                        {
                            if (choicesY != null)
                            {
                                hb.Th();
                            }
                            choicesX.ForEach(choice => hb
                                .Th(action: () => hb
                                    .HeaderText(
                                        context: context,
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        value: value,
                                        data: data.Where(o => o.GroupX == choice.Key),
                                        choice: choice)));
                        }))
                    .TBody(action: () =>
                    {
                        if (choicesY != null)
                        {
                            choicesY.ForEach(choiceY =>
                            {
                                hb.Tr(css: "kamban-row", action: () =>
                                {
                                    hb.Th(action: () => hb
                                        .HeaderText(
                                            context: context,
                                            ss: ss,
                                            aggregateType: aggregateType,
                                            value: value,
                                            data: data.Where(o => o.GroupY == choiceY.Key),
                                            choice: choiceY));
                                    choicesX.ForEach(choiceX => hb
                                        .Td(
                                            context: context,
                                            ss: ss,
                                            choiceX: choiceX.Key,
                                            choiceY: choiceY.Key,
                                            aggregateType: aggregateType,
                                            value: value,
                                            aggregationView: aggregationView,
                                            max: max,
                                            data: data,
                                            changedItemId: changedItemId));
                                });
                            });
                        }
                        else
                        {
                            hb.Tr(css: "kamban-row", action: () =>
                               choicesX.ForEach(choiceX => hb
                                    .Td(
                                        context: context,
                                        ss: ss,
                                        choiceX: choiceX.Key,
                                        choiceY: null,
                                        aggregateType: aggregateType,
                                        value: value,
                                        aggregationView: aggregationView,
                                        max: max,
                                        data: data,
                                        changedItemId: changedItemId)));
                        }
                    }));

        }

        private static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string choiceX,
            string choiceY,
            string aggregateType,
            Column value,
            bool aggregationView,
            decimal max,
            IEnumerable<KambanElement> data,
            long changedItemId)
        {
            return !aggregationView
                ? hb.Td(
                    attributes: new HtmlAttributes()
                        .Class("kamban-container")
                        .DataX(HttpUtility.HtmlEncode(choiceX))
                        .DataY(HttpUtility.HtmlEncode(choiceY)),
                    action: () => hb
                        .Div(action: () =>
                            data
                                .Where(o => o.GroupX == choiceX)
                                .Where(o => choiceY == null || o.GroupY == choiceY)
                                .ForEach(o => hb
                                    .Element(
                                        context: context,
                                        ss: ss,
                                        aggregateType: aggregateType,
                                        value: value,
                                        data: o,
                                        changedItemId: changedItemId))))
                : hb.Td(
                    context: context,
                    ss: ss,
                    choiceX: choiceX,
                    choiceY: choiceY,
                    aggregateType: aggregateType,
                    value: value,
                    aggregationView: aggregationView,
                    max: max,
                    data: data);
        }

        private static HtmlBuilder Td(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string choiceX,
            string choiceY,
            string aggregateType,
            Column value,
            bool aggregationView,
            decimal max,
            IEnumerable<KambanElement> data)
        {
            var num = data
                .Where(o => o.GroupX == choiceX)
                .Where(o => choiceY == null || o.GroupY == choiceY)
                .Summary(aggregateType);
            return hb.Td(action: () => hb
                .Text(text: value?.Display(
                    context: context,
                    value: num,
                    unit: aggregateType != "Count", format: aggregateType != "Count")
                        ?? num.ToString())
                .Svg(css: "svg-kamban-aggregation-view", action: () => hb
                    .Rect(
                        x: 0,
                        y: 0,
                        width: max > 0
                            ? (num / max * 100).ToString("0.0") + "%"
                            : "0",
                        height: "20px")));
        }

        private static HtmlBuilder HeaderText(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string aggregateType,
            Column value,
            IEnumerable<KambanElement> data,
            KeyValuePair<string, ControlData> choice)
        {
            return hb.Text(text: "{0}({1}){2}".Params(
                choice.Value.Text != string.Empty
                    ? choice.Value.Text
                    : Displays.NotSet(context: context),
                data.Count(),
                value != null && data.Any() && aggregateType != "Count"
                    ? " : " + value.Display(
                        context: context,
                        value: data.Summary(aggregateType),
                        unit: true)
                    : string.Empty));
        }

        private static decimal Summary(this IEnumerable<KambanElement> data, string aggregateType)
        {
            if (data.Any())
            {
                switch (aggregateType)
                {
                    case "Count": return data.Count();
                    case "Total": return data.Sum(o => o.Value);
                    case "Average": return data.Average(o => o.Value);
                    case "Min": return data.Min(o => o.Value);
                    case "Max": return data.Max(o => o.Value);
                    default: return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private static HtmlBuilder Element(
            this HtmlBuilder hb,
            Context context,
            SiteSettings ss,
            string aggregateType,
            Column value,
            KambanElement data,
            long changedItemId)
        {
            return hb.Div(
                attributes: new HtmlAttributes()
                    .Class("kamban-item" + ItemChanged(data.Id, changedItemId))
                    .DataId(data.Id.ToString()),
                action: () => hb
                    .Span(css: "ui-icon ui-icon-pencil")
                    .Text(text: ItemText(
                        context: context,
                        aggregateType: aggregateType,
                        value: value,
                        data: data)));
        }

        private static string ItemText(
            Context context, string aggregateType, Column value, KambanElement data)
        {
            return data.Title + (value == null || aggregateType == "Count"
                ? string.Empty
                : " : " + value.Display(
                    context: context,
                    value: data.Value,
                    unit: true));
        }

        private static string ItemChanged(long id, long changedItemId)
        {
            return id == changedItemId
                ? " changed"
                : string.Empty;
        }
    }
}