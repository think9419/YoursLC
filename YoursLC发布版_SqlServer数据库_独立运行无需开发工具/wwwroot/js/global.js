function showDetail(fwid, id, type) {
    if (type == 'select') {
        var width = '420px';
        if (layui.device().mobile) {
            width = '100%';
        }
        layer.open({
            type: 2,
            title: "查看数据",
            shade: 0.5,
            maxmin: true,
            area: [width, '150px'],
            anim: 2,
            content: "/" + fwid.replace("bi_", "").replace("fw_", "") + "/Detail?listid=" + id + ""
        });
    }
    if (type == 'pdf') {
        layer.open({
            type: 2,
            title: "查看数据",
            shade: 0.5,
            area: ['100%', '100%'],
            anim: 2,
            content: "/" + fwid.replace("bi_", "").replace("fw_", "") + "/GetDetail?listid=" + id + "&type=pdf&fwid=" + fwid
        });
    }
    if (type == 'html' || type == '_html') {
        layer.open({
            type: 2,
            title: "查看数据",
            shade: 0.5,
            area: ['100%', '100%'],
            anim: 2,
            maxmin: true,
            content: "/" + fwid.replace("bi_", "").replace("fw_", "") + "/GetDetail?listid=" + id + "&type=html&fwid=" + fwid
        });
    }
}

function replaceAll(content, searchValue, replaceValue) {
    while (content.indexOf(searchValue) > -1) {
        content = content.replace(searchValue, replaceValue);
    } return content;
}
function showDetailCom(fwid, id, type) {
    if (type == 'select') {
        var width = '420px';
        if (layui.device().mobile) {
            width = '100%';
        }
        layer.open({
            type: 2,
            title: "查看数据",
            shade: 0.5,
            maxmin: true,
            area: [width, '150px'],
            anim: 2,
            content: "/_TB/Detail?listid=" + id + "&fwid=" + fwid
        });
    }
    if (type == 'pdf') {
        layer.open({
            type: 2,
            title: "查看数据",
            shade: 0.5,
            area: ['100%', '100%'],
            anim: 2,
            content: "/Com/RDLCReport/ExportPdf?listid=" + id + "&fwid=" + fwid
        });
    }
    if (type == 'html' || type == '_html') {
        var high = '80%';
        if (!layui.device().mobile) {
            high = '80%';
        }
        layer.open({
            type: 2,
            title: "查看数据",
            shade: 0.5,
            maxmin: true,
            area: ['100%', high],
            anim: 2,
            maxmin: true,
            content: "/Com/RDLCReport/ExportHtml?listid=" + id + "&fwid=" + fwid
        });
    }
}

function showDate(date) {
    if (date == null || date == '') {
        return "";
    }
    else {
        return new Date(date).Format("yyyy-MM-dd");
    }
}

function showTime(time) {
    if (time == null || time == '') {
        return "";
    }
    else {
        return new Date(time).Format("yyyy-MM-dd hh:mm");
    }
}

// 替换 html 特殊字符
function replaceHtmlSymbol(html) {
    if (html == null) {
        return '';
    }
    return html.replace(/\n/g, ' ').replace(/"/gm, '&quot;').replace(/(\r\n|\r|\n)/g, '<br/>');
}

function replaceHtmlSymbol2(html) {
    if (html == null) {
        return '';
    }
    return html.replace(/</gm, '&lt;').replace(/>/gm, '&gt;').replace(/"/gm, '&quot;').replace(/(\r\n|\r|\n)/g, '<br/>');
}

function formatSql(sql) {
    sql = sql.replace(/&#x27;/g, "'");
    return sql;
}

function getMaxInt() {
    return "2147483647";
}

//图片显示
function showImg(id) {
    layer.open({
        type: 2,
        title: "图片查看",
        shade: 0.5,
        area: ['100%', '100%'],
        anim: 2,
        content: "/Com/FileManage/PictureShow?id=" + id + ""
    });
}

//子表图片上传
function popImgUplaod2(tbid, indexid, id, fname) {
    layer.open({
        type: 2,
        title: "图片上传",
        shade: 0.5,
        area: ['100%', '100%'],
        anim: 2,
        content: "/Com/FileManage/PictureUplaodGrid?tbid=" + tbid + "&indexid=" + tbid.replace(/tb_/, "") + "" + indexid + "&id=" + id + "&strv=" + indexid + "&fname=" + fname + "&from=add"
    });
}

//子表图片上传
function popFileUplaod(listid, fwid, tbid, indexid, id, fname) {
    layer.open({
        type: 2,
        title: "附件上传",
        shade: 0.5,
        area: ['300px', '200px'],
        anim: 2,
        content: "/Com/FileManage/FileUplaodGrid?listid=" + listid + "&fwid=" + fwid + "&tbid=" + tbid + "&indexid=" + tbid.replace(/tb_/, "") + "" + indexid + "&id=" + id + "&strv=" + indexid + "&fname=" + fname + "&from=add"
    });
}

//子表弹出框选择数据
function popSelectValue(tbid, indexid, id, url, fwid) {
    layer.open({
        type: 2,
        title: "选择数据",
        shade: 0.5,
        area: ['100%', '100%'],
        anim: 2,
        content: url + "tbid=" + tbid + "&indexid=" + tbid.replace(/tb_/, "") + "" + indexid + "&id=" + id + "&strv=" + indexid + "&from=add" + "&fwid=" + fwid
    });
}

function showIcon(icon) {
    return "<i class='ok-icon'>" + icon + "</i>";
}

function showIndexDetail(url) {
    layer.open({
        type: 2,
        title: "查看",
        shade: 0.5,
        area: ['500px', '350px'],
        anim: 2,
        content: url
    });
}

function openSetIcon() {
    layer.open({
        title: '选择图标',
        type: 2,
        area: ['100%', '100%'],
        fixed: false, //不固定
        maxmin: true,
        content: '/Icon.html'
    });
}

function getWebControls(_list) {
    var d = {};
    var t = $('#formUser [name]').serializeArray();
    $.each(t, function () {
        d[this.name] = this.value;
        var _row = { ControlValue: d[this.name], ControlID: this.value };
        _list.push(_row);
    });
    data = JSON.stringify(d);
}

// 时间格式方法
Date.prototype.Format = function (fmt) {
    if (this != null) {
        var o = {
            "M+": this.getMonth() + 1, //月份 

            "d+": this.getDate(), //日 

            "h+": this.getHours(), //小时 

            "m+": this.getMinutes(), //分 

            "s+": this.getSeconds(), //秒 

            "q+": Math.floor((this.getMonth() + 3) / 3), //季度 

            "S": this.getMilliseconds() //毫秒 
        };

        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));

        for (var k in o)

            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    }
    else {
        return '';
    }
}

Date.prototype.formatDate = function () { //author: meizz
    var myyear = this.getFullYear();
    var mymonth = this.getMonth() + 1;
    var myweekday = this.getDate();
    var myhour = this.getHours();
    var myminute = this.getMinutes();
    var second = this.getSeconds();
    if (mymonth < 10) {
        mymonth = "0" + mymonth;
    }
    if (myweekday < 10) {
        myweekday = "0" + myweekday;
    }
    if (myhour < 10) {
        myhour = "0" + myhour;
    }
    if (myminute < 10) {
        myminute = "0" + myminute;
    }
    if (second < 10) {
        second = "0" + second;
    }
    return (myyear.toString() + mymonth.toString() + myweekday.toString() + myhour.toString() + myminute.toString() + second.toString());
};