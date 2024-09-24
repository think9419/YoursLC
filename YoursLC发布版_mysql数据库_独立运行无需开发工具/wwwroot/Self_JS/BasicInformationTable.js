/**
 * Create:admin 2024-09-24 13:30:08
 * description:Think9企业级开发工具 http://yourslc.top 自定义扩展 此文件放置于wwwroot/Self_JS文件夹中
 */

//如何调用自定义函数
//1. 添加引用，如下
//layui.config({
// base: '/Self_JS/' /*自定义的js文件 wwwroot/Self_JS文件夹中*/
// });
//layui.use(["录入表编码去除tb_前缀"], function () {
//let myJS = layui.录入表编码去除tb_前缀;
//2. 调用函数，如myJS.要调用的函数名()

layui.define(['jquery', 'table'], function (exports) {
	let table = layui.table;
	let $ = layui.$;

	var api = {
	    //list页面，点击显示/隐藏查询条件
		searchShow: function () {
			var display = $('#searchfield').css('display');
			if (display == 'none') {
				$("#searchfield").show();
				document.body.scrollTop = document.documentElement.scrollTop = 0;
			}
			else {
				$("#searchfield").hide();
			}
		},
	    //编辑时锁定
		lockForEditing: function (_type) {
			//if (_type == 'edit') {
				//$('#要锁定的控件ID').attr('disabled','disabled');
			//}
		},
		//Form页面根据流程步骤编码或可编辑字段设置控件读写状态
		disabledControls: function (prcno, _fields, _hiddens) {
			//无流程 可编辑字段_fields不包含的视为禁用字段，禁用字段可设置
			//编码
			if (_fields != '#all#' && _fields.indexOf(',inCoding,') == -1) {
				$('#inCoding').attr('disabled','disabled');//禁用
				$('#inCoding').attr('class','layui-input layui-disabled');
				$('#inCoding').removeAttr('lay-verify');//去除校验
				$('#select_inCoding').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inCoding;') != -1) {
					$('#inCoding').attr('placeholder','******');//保密
					$('#inCoding').val('');
				}
			}
			//名称
			if (_fields != '#all#' && _fields.indexOf(',inName,') == -1) {
				$('#inName').attr('disabled','disabled');//禁用
				$('#inName').attr('class','layui-input layui-disabled');
				$('#inName').removeAttr('lay-verify');//去除校验
				$('#select_inName').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inName;') != -1) {
					$('#inName').attr('placeholder','******');//保密
					$('#inName').val('');
				}
			}
			//日期
			if (_fields != '#all#' && _fields.indexOf(',inDate,') == -1) {
				$('#inDate').attr('disabled','disabled');//禁用
				$('#inDate').attr('class','layui-input layui-disabled');
				$('#inDate').removeAttr('lay-verify');//去除校验
				$('#select_inDate').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inDate;') != -1) {
					$('#inDate').attr('placeholder','******');//保密
					$('#inDate').val('');
				}
			}
			//备注
			if (_fields != '#all#' && _fields.indexOf(',inRemarks,') == -1) {
				$('#inRemarks').attr('disabled','disabled');//禁用
				$('#inRemarks').attr('class','layui-input layui-disabled');
				$('#inRemarks').removeAttr('lay-verify');//去除校验
				$('#select_inRemarks').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inRemarks;') != -1) {
					$('#inRemarks').attr('placeholder','******');//保密
					$('#inRemarks').val('');
				}
			}
		},
		//控制List页面按钮状态 - 录入表管理/按钮管理可设置
        disableListBtn: function (strDisable) {
			layui.device().mobile ? $("#batchDel").html('<i class="fa fa-trash-o"></i>删除') : $("#batchDel").html('<i class="fa fa-trash-o"></i>删除');
			if (strDisable == 'all' || strDisable.indexOf("[batchDel]") > -1) {
				$("#batchDel").attr("class", "layui-btn layui-btn-disabled layui-btn-sm");//禁用批量删除
				$("#batchDel").removeAttr("lay-event");
				$("#batchDel_Mobile").attr("class", "layui-btn layui-btn-radius layui-btn-disabled layui-btn-sm");//禁用批量删除
				$("#batchDel_Mobile").removeAttr("lay-event");
			}

			layui.device().mobile ? $("#mergeExport").html('<i class="fa fa-file-pdf-o"></i>导出') : $("#mergeExport").html('<i class="fa fa-file-pdf-o"></i>导出');
			if (strDisable == 'all' || strDisable.indexOf("[mergeExport]") > -1) {
				$("#mergeExport").attr("class", "layui-btn layui-btn-disabled layui-btn-sm");//禁用批量导出
				$("#mergeExport").removeAttr("lay-event");
				$("#mergeExport_Mobile").attr("class", "layui-btn layui-btn-radius layui-btn-disabled layui-btn-sm");//禁用批量导出
				$("#mergeExport_Mobile").removeAttr("lay-event");
			}

			layui.device().mobile ? $("#importExcel").html('<i class="fa fa-file-excel-o"></i>导入') : $("#importExcel").html('<i class="fa fa-file-excel-o"></i>导入');
			if (strDisable == 'all' || strDisable.indexOf("[importExcel]") > -1) {
				$("#importExcel").attr("class", "layui-btn layui-btn-disabled layui-btn-sm");//禁用数据导入
				$("#importExcel").removeAttr("lay-event");
				$("#importExcel_Mobile").attr("class", "layui-btn layui-btn-radius layui-btn-disabled layui-btn-sm");//禁用数据导入
				$("#importExcel_Mobile").removeAttr("lay-event");
			}

			layui.device().mobile ? $("#add").html('<i class="fa fa-plus"></i>新增') : $("#add").html('<i class="fa fa-plus"></i>新增');
			if (strDisable == 'all' || strDisable.indexOf("[add]") > -1) {
				$("#add").attr("class", "layui-btn layui-btn-disabled layui-btn-sm");//禁用数据新增
				$("#add").removeAttr("lay-event");
			}


		},
		//控制Form页面按钮状态 - 录入表管理/按钮管理可设置
		disableFormBtn: function (strDisable) {
            if (strDisable == 'all' || strDisable.indexOf("[print]") > -1) {
                $("#print").attr("class", "layui-btn layui-btn-disabled");//禁用打印
				$("#print").removeAttr("href");
            }
            if (strDisable == 'all' || strDisable.indexOf("[att]") > -1) {
                $("#att").attr("class", "layui-btn layui-btn-disabled");//禁用附件
				$('#att').unbind('click');
            }
            if (strDisable == 'all' || strDisable.indexOf("[edit]") > -1) {
                $("#edit").attr("class", "layui-btn layui-btn-disabled");//禁用保存
                $("#edit").removeAttr("lay-filter");
            }
            if (strDisable == 'all' || strDisable.indexOf("[next]") > -1) {
                $("#next").attr("class", "layui-btn layui-btn-disabled");//禁用转交
                $("#next").removeAttr("lay-filter");
            }
            if (strDisable == 'all' || strDisable.indexOf("[finish]") > -1) {
                $("#finish").attr("class", "layui-btn layui-btn-disabled");//禁用结束
                $("#finish").removeAttr("lay-filter");
            }
		},
		//控制子表纵列页面控件状态
        disableGridColumnFormControl: function (grid, strDisable) {
			//无子表
        },
		//将所有主表控件（ID、Value和ControlType）Push到list中
		getControlValueList: function () {
		    var _list = [];
			_list.push({ ControlID: 'listid', ControlValue: $('#_listid').val(), ControlType: '1' });
			_list.push({ ControlID: 'inCoding', ControlValue: $('#inCoding').val(), ControlType: '1' });//编码
			_list.push({ ControlID: 'inName', ControlValue: $('#inName').val(), ControlType: '1' });//名称
			_list.push({ ControlID: 'inDate', ControlValue: $('#inDate').val(), ControlType: '1' });//日期
			_list.push({ ControlID: 'inRemarks', ControlValue: $('#inRemarks').val(), ControlType: '1' });//备注
			return _list;
		},
		//将所有主表控件（ID、Value和ControlType）Push到list中--从父页面获取
		getControlValueListFrmParent: function () {
		    var _list = [];
			_list.push({ ControlID: 'listid', ControlValue: $('#_listid').val(), ControlType: '1' });
			_list.push({ ControlID: 'inCoding', ControlValue: $(window.parent.document).find('#inCoding').val(), ControlType: '1' });//编码
			_list.push({ ControlID: 'inName', ControlValue: $(window.parent.document).find('#inName').val(), ControlType: '1' });//名称
			_list.push({ ControlID: 'inDate', ControlValue: $(window.parent.document).find('#inDate').val(), ControlType: '1' });//日期
			_list.push({ ControlID: 'inRemarks', ControlValue: $(window.parent.document).find('#inRemarks').val(), ControlType: '1' });//备注
			return _list;
		},
		//子表纵列页面将所有控件（ID、Value和ControlType）Push到list中
		getControlListFrmGridColumn: function (grid) {
		    var _list = [];
			_list.push({ ControlID: 'listid', ControlValue: $('#_listid').val(), ControlType: '1' });
			//无子表
			return _list;
		},
		//执行数据读取--根据后台返回的lists，为控件赋值
		setValueByList: function (lists, _tbid, _rowid) {
		    //主表或者子表纵列
			if (_tbid == '_main' || _tbid == '_gridColumn') {//
				for (var item in lists) {
					var _value = lists[item].ControlValue;//控件值
					var _id = lists[item].ControlID;//控件id
					var _type = lists[item].ControlType;//控件类型
					var _list = lists[item].ListValue;//列表值，数据联动时需要
					if (_list == null) {//读取单值
						//text文本框
						if (_type == "1") {
							$("#" + _id).val(_value);
						}
						//select下拉选择
						if (_type == "2") {
							$("select[name=" + _id + "]").val(_value);
						}
						//checkbox复选框
						if (_type == "3") {
							$("#" + _id + "_Exa").val(_value);
							$("input[name=" + _id + "]").each(function () {

								if (_value.indexOf($('#_plit').val() + $(this).val() + $('#_plit').val()) != -1) {
									$(this).prop('checked', true);
								}
								else {
									$(this).prop('checked', false);
								}
							});
						}
						//radio单选框
						if (_type == "4") {
							$("input[name=" + _id + "]").each(function () {
								if ($(this).val() == _value) {
									$(this).prop('checked', true);
								}
								else {
									$(this).prop('checked', false);
								}
							});
						}
						//img图片
						if (_type == "5") {
							if (_value == '' || _value == null) {
								$("#" + _id + "_Exa").val('nonexistent.gif');
								$("#" + _id).attr('src', '/images/nonexistent.gif');
								$("#" + _id).attr('alt', '');
							}
							else {
								$("#" + _id + "_Exa").val(_value);
								$("#" + _id).attr('src', '/UserImg/' + _value);
								$("#" + _id).attr('alt', _value);
							}
						}
					}
					else {//数据联动
						var _controlid = "#" + _id;
						$("" + _controlid + "").empty(); //清空控件
					    if (_type == "2") {
                            $("" + _controlid + "").append(new Option('==请选择==', ''));
                        }
						$.each(_list, function (i, item) {
							$("" + _controlid + "").append(new Option(item.Text, item.Value));
						});
					}
				}
			}
            else {//子表
                var _starts = "#" + _tbid + "#" + _rowid + "#";
                var trList = $(".layui-table").find("tr");
                for (var i = 0; i < trList.length; i++) {
                    var tdArr = trList.eq(i).find("td");
					//无子表
                }
            }
		},
		//弹出选择触发--弹出页面关闭后，将选择的Value赋值给触发弹出页面的input
		//_tbid 表id，主表为_main，子表纵列为_gridColumn
		//_indexid 指标编码
		//_id 子表数据主键
		//_v 第几列--子表弹出时有用
		//_value 弹出页面选择的值
		getValueFromPopUp: function (_tbid, _indexid, _id, _v, _value) {
			var flag='';
			if (_tbid == '_main' || _tbid == '_gridColumn') {//主表或者子表纵列
                if (_tbid == '_gridColumn') {//子表纵列
					var lastTwo = _indexid.substring(_indexid.length - 2);
					if (lastTwo == 'v1' || lastTwo == 'v2' || lastTwo == 'v3' || lastTwo == 'v4' || lastTwo == 'v5' || lastTwo == 'v6' || lastTwo == 'v7' || lastTwo == 'v8' || lastTwo == 'v9') {
						$("#" + _indexid.slice(-2) + "").val(_value);//v1-v9
					}
					else {
						$("#" + _indexid.slice(-3) + "").val(_value);//v10-v99
					}
                }
                else {
                    $("#" + _indexid + "").val(_value);
                }
			}
			else {
			    var flag = '';
				var _starts = "#" + _tbid + "#" + _id + "#";
				var trList = $(".layui-table").find("tr");
				for (var i = 0; i < trList.length; i++) {
					var tdArr = trList.eq(i).find("td");
					flag = tdArr.eq(0).text();
					//无子表
				}
			}
		},
		//只取子表数据首行
		getFirstGridTable: function () {
		    var _list = [];
			var trList = $(".layui-table").find("tr");  //获取table下的所有tr
			for (var i = 0; i < trList.length; i++) {  //遍历所有的tr
				var tdArr = trList.eq(i).find("td"); //获取该tr下的所有td
				var flag = tdArr.eq(0).text();
					//无子表
			}
			return _list;
		},
		//取子表当前行数据
		getCurrentGridTable: function (tbid, id) {
		    var _list = [];
			var trList = $(".layui-table").find("tr");  //获取table下的所有tr
			for (var i = 0; i < trList.length; i++) {  //遍历所有的tr
				var tdArr = trList.eq(i).find("td"); //获取该tr下的所有td
				var flag = tdArr.eq(0).text();
				//无子表
			}
			return _list;
		},
		//遍历子表取数--可从多个子表取值
		foreachGridTable: function () {
		    var _list = [];
			var trList = $(".layui-table").find("tr");  //获取table下的所有tr
			for (var i = 0; i < trList.length; i++) {  //遍历所有的tr
				var tdArr = trList.eq(i).find("td"); //获取该tr下的所有td
				var flag = tdArr.eq(0).text();
				//无子表
			}
			return _list;
		},
		//设置子表不可编辑
		disabledGridTable: function (grid) {

			var trList = $(".layui-table").find("tr");  //获取table下的所有tr
			for (var i = 0; i < trList.length; i++) {  //遍历所有的tr
				var tdArr = trList.eq(i).find("td"); //获取该tr下的所有td
				var flag = tdArr.eq(0).text();
				//无子表
			}
		},
		previewImg: function (src) {
			var img = new Image();
			img.src = src;
			var height = 200; //获取图片高度
			var width = 200; //获取图片宽度
			var imgHtml = "<img src='" + src + "' style='width: " + width + "px;height:" + height + "px'/>";
			layer.open({
				type: 1,
				offset: 'auto',
				area: [width + 'px', height + 'px'],
				shadeClose: true,//点击外围关闭弹窗
				scrollbar: true,//不现实滚动条
				title: false, //不显示标题
				content: imgHtml, //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响
				cancel: function () {
				}
			});
		}

	};
	//暴露接口
	exports('BasicInformationTable', api);
});