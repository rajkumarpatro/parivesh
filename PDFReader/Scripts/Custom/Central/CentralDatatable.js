var CentralTables = function () {
    //TODO:: table-datatables-scroller.js use to fix scroller issue
    var table;
    var Keyword = function () {
        debugger;
        table = $('#CentralTable').dataTable({
            retrieve: true,
            "autowidth": "true",
            "scrollX": true,
            "ajax": {
                "url": searchedCentral,
                "type": "GET",
                "datatype": "json"
            },
            "columns": [
                { "title": "DateTimeStamp", "data": "DateTimeStamp" },
                { "title": "PRNo", "data": "Proposal_No" },
                { "title": "File", "data": "MOEFCC_File_No" },
                { "title": "Project", "data": "Project_Name" },
                { "title": "Company", "data": "Company" },
                { "title": "Status", "data": "Proposal_Status" },
                { "title": "Location", "data": "Location" },
                { "title": "ImpDates", "data": "Important_Dates" },
                { "title": "Category", "data": "Category" },
                { "title": "Type", "data": "Type_of_project" },
                { "title": "Files", "data": "Attached_Files" },
                { "title": "Acknowlegment", "data": "Acknowlegment" },
                { "title": "Input", "data": "input_company_name" },
                { "title": "subsidiary_name", "data": "subsidiary_name" }
                
                //{
                //    "title": "Url", "data": "Url",
                //    fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                //        oData.Url = oData.Url + " ";
                //        $(nTd).html("<a href='" + oData.Url + "' target='_blank'>pdf page</a>");
                //    }
                //},
                //{ "title": "Year", "data": "FinancialYear" },
                //{ "title": "Page Number", "data": "PDFPageNumber" },
                //{ "title": "Keywords", "data": "FoundKeywords" },
                //{ "title": "Total Pages", "data": "TotalPages" },
                //{
                //    "title": "", "data": "TotalPages",
                //    fnCreatedCell: function (nTd, sData, oData, iRow, iCol) {
                //        oData.Url = oData.Url + " ";
                //        $(nTd).html("<a href='/Central/advancesearch?Centralid=" + oData.CentralId + "' target='_blank'><i class='feather icon-search'></i></a>");
                //    } }
            ],
            "sAjaxDataProp": "",
            dom: 'lBfrtip',
            buttons: ['excel', 'csv'],
            iDisplayLength: 100,
            "aLengthMenu": [[100, 200, -1], [100, 200, "All"]],
        });
    }

    return {
        //main function to initiate the module
        init: function () {
            if (!jQuery().dataTable) {
                return;
            }

            Keyword();
        },
        destroy: function () {
            if (table != null)
                table.DataTable().destroy();
        },
        clear: function () {
            if (table != null)
                table.DataTable().clear();
        },
        draw: function () {
            if (table != null)
                table.DataTable().draw();
        },
        reloadTable: function () {
            if (table != null)
                table.DataTable().ajax.reload(null, false); // user paging is not reset on reload
        }
    };
}();