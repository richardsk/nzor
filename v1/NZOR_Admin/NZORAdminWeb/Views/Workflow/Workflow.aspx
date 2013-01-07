<%@ Page Title="" Language="VB" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Workflow
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript" language="javascript">

        $(document).ready(function (event) {
            $('#importButton').click(function () {
                $.post("/Workflow/ImportProviderData", {}, function () {
                    $('#importStatusBorder').show();
                    getStatus("Import", "#importStatusFill", "#importStatus");
                });
                event.preventDefault;
            });

            $('#integButton').click(function () {
                if (document.getElementById("integButton").value == "Run Integration") {
                    $.post("/Workflow/RunIntegration", {}, function () {
                        document.getElementById("integButton").value = "Stop Integration";
                        $('#integStatus').html('Processing ...');
                        $('#integStatusBorder').show();
                        getStatus("Integration", "#integStatusFill", "#integStatus");
                    });
                }
                else {
                    $.post("/Workflow/StopIntegration", {}, function () {
                        document.getElementById("integButton").value = "Run Integration";
                        $('#integStatusBorder').hide();
                    });
                }
                event.preventDefault;
            });

            $('#refreshButton').click(function () {
                $.post("/Workflow/RunConsensusRefresh", {}, function () {
                    $('#refreshStatusBorder').show();
                    getStatus("ConsensusRefresh", "#refreshStatusFill", "#refreshStatus");
                });
                event.preventDefault;
            });

            $('#webCacheButton').click(function () {
                $.post("/Workflow/RunWebCacheRefresh", {}, function () {
                    $('#webCacheStatusBorder').show();
                    getStatus("WebCacheRefresh", "#webCacheStatusFill", "#webCacheStatus");
                });
                event.preventDefault;
            });
        });

        
        function getStatus(action, progBar, status) {
            var _action = action.toString();
            var _progBar = progBar.toString();
            var _status = status.toString();
            
            var url = '/Workflow/GetProgress/' + _action;
            $.get(url, function (data) {
                var strs = data.toString().split("|");
                if (strs[0] != "100") {
                    $(_progBar.toString()).width(strs[0] * 2);
                    $(_status.toString()).html(strs[1]);
                    window.setTimeout(function () { getStatus(_action, _progBar, _status); }, 1000);
                }
                else {
                    $('#integStatusBorder').hide();
                };
            });            
        }        
        
    </script>


    <br />
    <h2>Workflow</h2>
    
    <fieldset>
        <legend>Import Provider Data</legend>
        
        Provider data file (NZOR XML) :
        
        <input id="providerDataFile" style="width: 70%" type="file" /><br />
        
        <input id="importButton" type="submit" value="Import"/><br />
         <p>
        <div id="importStatusBorder" class="statusBorder">
            <div id="importStatusFill" class="statusFill">
            </div>
        </div>
        <div id="importStatus" class="body" ></div>
        </p>

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
        <div id="integStatus" class="body" ></div>
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
        <div id="refreshStatus" class="body" ></div>
        </p>
    </fieldset>
    
    <fieldset>
        <legend>Refresh Web Cache</legend>
                        
        <input id="webCacheButton" type="submit" value="Run Refresh" /><br />        
        <p>
        <div id="webCacheStatusBorder" class="statusBorder">
            <div id="webCacheStatusFill" class="statusFill">
            </div>
        </div>
        <div id="webCacheStatus" class="body" ></div>
        </p>
    </fieldset>

   
    <br />
    

</asp:Content>
