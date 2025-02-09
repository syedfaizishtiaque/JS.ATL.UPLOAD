﻿var tblFBRFile = null;
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
    $.ajax({
        url: `/Home/UploadFile`,
        data: { file_name: filename },
        method: 'GET',
        success: function (data) {
            alert(data);
        },
        error: function (error) {
            alert('Failed to upload File');
        }
    });
}
function DownloadFileByPath(filename) {
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
        }
        else {
            alert('Failed to download File');
        }
    };

    xhr.send();
}


