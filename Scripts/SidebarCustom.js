function GetTree() {
    var data;
    if (localStorage.getItem("menu") != null && localStorage.getItem("menu") != '') {

        $('#sidebarcustom').append(localStorage.getItem("menu"));
        return;
    }
    $.ajax(
                       {
                           url: '/Search/GetMenu/',
                           type: "POST",
                           contentType: "application/json; charset=utf-8",
            //dataType: "json",
            success: function (response) {
                               // data = JSON.parse(response.data);
               // console.log(response);
                              // loadTree(response);
                           },
            error: function (response) {
                console.log(response);
                           }
                          

                       });





}

function loadTree(data) {

    var kl = 0;
    var km = 0;
    var kn = 0;
    var temp = false;
    var html = "";
    var parentnode = [];
    for (var i = 0; i < data.length; i++) {
        if (data[i].parent_mnu_sk == 0) {
            parentnode.push(data[i])

        }
    }
    for (i = 0; i < parentnode.length; i++) {
        html +=
                                    '<li class="nav-item"><a class="nav-link" href="' + parentnode[i].mnu_url + '">' +
                                   '<i class="' + parentnode[i].mnu_icon + '"></i>' +
                                   '<p> ' + parentnode[i].mnu_desc + '</p>' +
                                    //'<i class="fa fa-angle-left pull-right"></i>' +
                                  "</a>";//wraper
        pid = parentnode[i].mnu_sk;
        for (j = 0; j < data.length; j++) {
            ///alert(data[j].parent_mnu_sk);

            if (pid == data[j].parent_mnu_sk) {

                if (kl == 0) {
                    html += '<ul class="nav nav-treeview" >'; kl++;
                }

                html += '<li class="nav-item">' +

                                 '<a class="nav-link" href="' + data[j].mnu_url + '"><i class="fa fa-circle-o"></i>';

                //'' + data[j].mnu_desc + '<i class="fa fa-angle-left pull-right"></i></a>';
                level1 = data[j].mnu_sk;
                for (k = 0; k < data.length; k++) {
                    if (level1 == data[k].parent_mnu_sk) {
                        if (km == 0) {
                            html += '<ul class="nav nav-treeview" >'; km++;
                        }

                        html += '<li class="nav-item">' +
                       ' <a class="nav-link" href="' + data[k].mnu_url + '"><i class="fa fa-circle-o"></i><p> ' + data[k].mnu_desc + '</p></a>';

                        level2 = data[k].mnu_sk;
                        for (l = 0; l < data.length; l++) {
                            if (level2 == data[l].parent_mnu_sk) {
                                temp = true;
                                if (kn == 0) {
                                    html += '<ul class="nav nav-treeview" >'; kn++;
                                }
                                html += '<li class="nav-item">' +
                             '<a class="nav-link" href="' + data[l].mnu_url + '"><i class="fa fa-circle-o"></i> <p>' + data[l].mnu_desc +

                             '</p></a>' +
                                 "</li>";

                            }

                        }
                        if (kn == 1) {
                            html += '</ul>';
                        }
                        html += '</li>';

                        kn = 0;
                    }


                }
                if (km == 1) {
                    html += '</ul>';
                }
                html += '</li>';
            }



            km = 0;
        }
        if (kl == 1) {
            html += '</ul>';
        }//close 2nd level
        html += '</li>'//parent node close i.e end module
        kl = 0;

    }

    $('#sidebarcustom').append(html);

    localStorage.setItem("menu", html);
}








