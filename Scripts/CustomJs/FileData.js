var tblFBRFile = null;
var tblFileLog = null;
$(document).ready(function () {
    LoadFilesData();
    LoadFileLog();
    setInterval(function () { tblFileLog.ajax.reload(); }, 1000);
});


function LoadFilesData() {
    tblFBRFile = $('#tblFBRFile').DataTable({

        "paging": true,
        "lengthChange": false,
        "searching": true,
        "ordering": true,
        ajax: {
            //url: "/ATL.UPLOAD/Home/ShowProcessedFile",
            url: "/Home/ShowProcessedFile",
            data: '',
            dataType: 'json',
            type: 'GET',
            dataSrc: '',
        },
        "columns": [

            {
                data: null,
                render: function (data, type, full, meta) {

                    return meta.row + 1;
                }
            },
            { "data": "FileName" },

            { "data": "FileDate" },
            {
                data: null,
                render
                    : function (data, type, full, meta) {

                        return '<button type="button" target="_blank" title="Upload File on T24" onclick="UploadFile(\'' + data.FileName + '\');" class="btn btn-default btn-sm"><i class="fas fa-upload"></i></button>';

                    }
            },
            {
                data: null,
                render
                    : function (data, type, full, meta) {

                        return '<button type="button" target="_blank" title="Download" onclick="DownloadFileByPath(\'' + data.FileName + '\');" class="btn btn-default btn-sm"><i class="fas fa-download"></i></button>';

                    }
            }
        ]

    });
}
function LoadFileLog() {
    tblFileLog=  $('#tblFileLog').DataTable({

        "paging": false,
        "lengthChange": false,
        "searching": true,
        "ordering": true,
        ajax: {
            //url: "/ATL.UPLOAD/Home/ShowProcessedFileLog",
            url: "/Home/ShowProcessedFileLog",
            data: '',
            dataType: 'json',
            type: 'GET',
            dataSrc: '',
        },
        "columns": [

            {
                data: null,
                render: function (data, type, full, meta) {
                    if (data.ProcessLog == "Process Completed") {
                        tblFBRFile.ajax.reload();
                    }
                    return meta.row + 1;
                }
            },
            { "data": "ProcessLog" },

           
        ]

    });
}


function UploadFile(filename) {
    ShowLoader();
    $.ajax({
        url: `/Home/UploadFile`,
        data: { file_name: filename },
        method: 'GET',
        success: function (data) {
            HideLoader();
            alert(data);
        },
        error: function (error) {
            HideLoader();
            alert('Failed to upload File');
        }
    });
}
function DownloadFileByPath(filename) {
    ShowLoader();
    var xhr = new XMLHttpRequest();
    //xhr.open('GET', '/ATL.UPLOAD/Home/DownloadFileByPath?fileName=' + filename);
    xhr.open('GET', '/Home/DownloadFileByPath?fileName=' + filename);
    xhr.responseType = 'blob';  
    xhr.onload = function () {
        console.log('check point ');
        console.log(xhr.response);
        if (xhr.status === 200) {
            // Create a link element and trigger the download
            var blob = new Blob([xhr.response], { type: "text/csv"  });
          
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = filename;
            link.click();
            HideLoader();
        }
        else {
            HideLoader();
            alert('Failed to download File');
            
        }
    };

    xhr.send();
   
}

function ShowLoader() {
    $("#preloaderjq").removeAttr("style");
    $("#preloaderjq").attr("style", "background:none!important;")
    //$("#preloaderjq").css("background","none!important");
    //$("#preloaderjq").css("height", "auto!important");
    $("#imgloader").removeAttr("style");
    $("#imgloader").attr("style", "display:block!important");
}
function HideLoader() {
    $("#preloaderjq").removeAttr("style");
    //$("#preloaderjq").css("height", "0px!important");
    //$("#preloaderjq").css("background", "none!important");
    $("#preloaderjq").attr("style", "background:none!important;height:0px!important;")
    $("#imgloader").removeAttr("style");
    //$("#imgloader").css("display", "none!important");
    $("#imgloader").attr("style", "display:none!important");
}


