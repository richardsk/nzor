<%@ Page Title="" Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Workflow
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">
    
        $(document).ready(function (event) {
            $('#integButton').click(function () {
                $.post("/Workflow/RunIntegration", {}, function () {
                    $('#integStatusBorder').show();
                    getStatus("Integration", "#integStatusFill");
                });
                event.preventDefault;
            });
            
            $('#refreshButton').click(function () {
                $.post("/Workflow/RunConsensusRefresh", {}, function () {
                    $('#refreshStatusBorder').show();
                    getStatus("ConsensusRefresh", "#refreshStatusFill");
                });
                event.preventDefault;
            });
        });


        function getStatus(action, progBar) {
            var url = '/Workflow/GetProgress/' + action;
            $.get(url, function (data) {
                if (data != "100") {
                    $(progBar).width(data);
                    var code = "getStatus(" + action + "," + progBar + ")";
                    window.setTimeout(code, 1000);
                    }
                else {
                    $(progBar).hide();
                };
            });
        }        
        
    </script>


    <%Using Html.BeginForm%>

    <br />
    <h2>Workflow</h2>
    
    <fieldset>
        <legend>Import Provider Data</legend>
        
        Provider data file (NZOR XML) :
        
        <input id="File1" style="width: 70%" type="file" /><br />
        
        <input id="importButton" type="submit" value="Import"/><br />
        <br />

    </fieldset>
    <br />
    
    <fieldset>
        <legend>Integration</legend>
                        
        <input id="integButton" type="submit" value="Run Integration" /><br />        
        <p>
        <div id="integStatusBorder" class="statusBorder">
            <div id="integStatusFill" class="statusFill">
            </div>
        </div>
        </p>
    </fieldset>
    
    <fieldset>
        <legend>Refresh Consensus</legend>
                        
        <input id="refreshButton" type="submit" value="Run Refresh" /><br />        
        <p>
        <div id="refreshStatusBorder" class="statusBorder">
            <div id="refreshStatusFill" class="statusFill">
            </div>
        </div>
        </p>
    </fieldset>

   
    <br />
    
    <% End Using%>

</asp:Content>
