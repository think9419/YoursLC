﻿/**
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
			//小数01
			if (_fields != '#all#' && _fields.indexOf(',inXS01,') == -1) {
				$('#inXS01').attr('disabled','disabled');//禁用
				$('#inXS01').attr('class','layui-input layui-disabled');
				$('#inXS01').removeAttr('lay-verify');//去除校验
				$('#select_inXS01').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inXS01;') != -1) {
					$('#inXS01').attr('placeholder','******');//保密
					$('#inXS01').val('');
				}
			}
			//小数02
			if (_fields != '#all#' && _fields.indexOf(',inXS02,') == -1) {
				$('#inXS02').attr('disabled','disabled');//禁用
				$('#inXS02').attr('class','layui-input layui-disabled');
				$('#inXS02').removeAttr('lay-verify');//去除校验
				$('#select_inXS02').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inXS02;') != -1) {
					$('#inXS02').attr('placeholder','******');//保密
					$('#inXS02').val('');
				}
			}
			//小数03
			if (_fields != '#all#' && _fields.indexOf(',inXS03,') == -1) {
				$('#inXS03').attr('disabled','disabled');//禁用
				$('#inXS03').attr('class','layui-input layui-disabled');
				$('#inXS03').removeAttr('lay-verify');//去除校验
				$('#select_inXS03').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inXS03;') != -1) {
					$('#inXS03').attr('placeholder','******');//保密
					$('#inXS03').val('');
				}
			}
			//小数04
			if (_fields != '#all#' && _fields.indexOf(',inXS04,') == -1) {
				$('#inXS04').attr('disabled','disabled');//禁用
				$('#inXS04').attr('class','layui-input layui-disabled');
				$('#inXS04').removeAttr('lay-verify');//去除校验
				$('#select_inXS04').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inXS04;') != -1) {
					$('#inXS04').attr('placeholder','******');//保密
					$('#inXS04').val('');
				}
			}
			//小数05
			if (_fields != '#all#' && _fields.indexOf(',inXS05,') == -1) {
				$('#inXS05').attr('disabled','disabled');//禁用
				$('#inXS05').attr('class','layui-input layui-disabled');
				$('#inXS05').removeAttr('lay-verify');//去除校验
				$('#select_inXS05').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inXS05;') != -1) {
					$('#inXS05').attr('placeholder','******');//保密
					$('#inXS05').val('');
				}
			}
			//小数06
			if (_fields != '#all#' && _fields.indexOf(',inXS06,') == -1) {
				$('#inXS06').attr('disabled','disabled');//禁用
				$('#inXS06').attr('class','layui-input layui-disabled');
				$('#inXS06').removeAttr('lay-verify');//去除校验
				$('#select_inXS06').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inXS06;') != -1) {
					$('#inXS06').attr('placeholder','******');//保密
					$('#inXS06').val('');
				}
			}
			//小数07
			if (_fields != '#all#' && _fields.indexOf(',inXS07,') == -1) {
				$('#inXS07').attr('disabled','disabled');//禁用
				$('#inXS07').attr('class','layui-input layui-disabled');
				$('#inXS07').removeAttr('lay-verify');//去除校验
				$('#select_inXS07').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inXS07;') != -1) {
					$('#inXS07').attr('placeholder','******');//保密
					$('#inXS07').val('');
				}
			}
			//小数08
			if (_fields != '#all#' && _fields.indexOf(',inXS08,') == -1) {
				$('#inXS08').attr('disabled','disabled');//禁用
				$('#inXS08').attr('class','layui-input layui-disabled');
				$('#inXS08').removeAttr('lay-verify');//去除校验
				$('#select_inXS08').unbind('click');//解除点击选择
				if (_hiddens.indexOf(';inXS08;') != -1) {
					$('#inXS08').attr('placeholder','******');//保密
					$('#inXS08').val('');
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
			if (grid =='tb_Subtable05') {
				if (strDisable.indexOf('[v1]') > -1) {
					$('#v1').attr('disabled', 'disabled');//列12儿童
					$('#v1').removeAttr('lay-verify');//列12儿童
					$('#v1').attr('class', 'layui-input layui-disabled')
					$('#select_v1').unbind('click');
				}
				if (strDisable.indexOf('[v2]') > -1) {
					$('#v2').attr('disabled', 'disabled');//列2
					$('#v2').removeAttr('lay-verify');//列2
					$('#v2').attr('class', 'layui-input layui-disabled')
					$('#select_v2').unbind('click');
				}
				if (strDisable.indexOf('[v3]') > -1) {
					$('#v3').attr('disabled', 'disabled');//列3
					$('#v3').removeAttr('lay-verify');//列3
					$('#v3').attr('class', 'layui-input layui-disabled')
					$('#select_v3').unbind('click');
				}
			}
        },
		//将所有主表控件（ID、Value和ControlType）Push到list中
		getControlValueList: function () {
		    var _list = [];
			_list.push({ ControlID: 'listid', ControlValue: $('#_listid').val(), ControlType: '1' });
			_list.push({ ControlID: 'inXS01', ControlValue: $('#inXS01').val(), ControlType: '1' });//小数01
			_list.push({ ControlID: 'inXS02', ControlValue: $('#inXS02').val(), ControlType: '1' });//小数02
			_list.push({ ControlID: 'inXS03', ControlValue: $('#inXS03').val(), ControlType: '1' });//小数03
			_list.push({ ControlID: 'inXS04', ControlValue: $('#inXS04').val(), ControlType: '1' });//小数04
			_list.push({ ControlID: 'inXS05', ControlValue: $('#inXS05').val(), ControlType: '1' });//小数05
			_list.push({ ControlID: 'inXS06', ControlValue: $('#inXS06').val(), ControlType: '1' });//小数06
			_list.push({ ControlID: 'inXS07', ControlValue: $('#inXS07').val(), ControlType: '1' });//小数07
			_list.push({ ControlID: 'inXS08', ControlValue: $('#inXS08').val(), ControlType: '1' });//小数08
			return _list;
		},
		//将所有主表控件（ID、Value和ControlType）Push到list中--从父页面获取
		getControlValueListFrmParent: function () {
		    var _list = [];
			_list.push({ ControlID: 'listid', ControlValue: $('#_listid').val(), ControlType: '1' });
			_list.push({ ControlID: 'inXS01', ControlValue: $(window.parent.document).find('#inXS01').val(), ControlType: '1' });//小数01
			_list.push({ ControlID: 'inXS02', ControlValue: $(window.parent.document).find('#inXS02').val(), ControlType: '1' });//小数02
			_list.push({ ControlID: 'inXS03', ControlValue: $(window.parent.document).find('#inXS03').val(), ControlType: '1' });//小数03
			_list.push({ ControlID: 'inXS04', ControlValue: $(window.parent.document).find('#inXS04').val(), ControlType: '1' });//小数04
			_list.push({ ControlID: 'inXS05', ControlValue: $(window.parent.document).find('#inXS05').val(), ControlType: '1' });//小数05
			_list.push({ ControlID: 'inXS06', ControlValue: $(window.parent.document).find('#inXS06').val(), ControlType: '1' });//小数06
			_list.push({ ControlID: 'inXS07', ControlValue: $(window.parent.document).find('#inXS07').val(), ControlType: '1' });//小数07
			_list.push({ ControlID: 'inXS08', ControlValue: $(window.parent.document).find('#inXS08').val(), ControlType: '1' });//小数08
			return _list;
		},
		//子表纵列页面将所有控件（ID、Value和ControlType）Push到list中
		getControlListFrmGridColumn: function (grid) {
		    var _list = [];
			_list.push({ ControlID: 'listid', ControlValue: $('#_listid').val(), ControlType: '1' });
			if (grid =='tb_Subtable05') {
				_list.push({ ControlID: 'v1', ControlValue: $('#v1').val(), ControlType: '1' });//列12儿童
				_list.push({ ControlID: 'v2', ControlValue: $('#v2').val(), ControlType: '1' });//列2
				_list.push({ ControlID: 'v3', ControlValue: $('#v3').val(), ControlType: '1' });//列3
			}
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
					if (tdArr.eq(0).text().startsWith(_starts) && _tbid == 'tb_Subtable05') {
						for (var item in lists) {
							var _list = lists[item].ListValue;
							var _value = lists[item].ControlValue;
							var _id = lists[item].ControlID.replace("v", "");//列序号如1、2、3...
							if (_list == null) {//读取单值
								if (lists[item].ControlType == "2") {
									tdArr.eq(parseInt(_id) + 1).find('select').val(_value);//下拉选择 错位（有序号）
								}
								else {
									tdArr.eq(parseInt(_id) + 1).find('input').val(_value);//错位（有序号）
								}
							}
							else {//数据联动
								if (lists[item].ControlType == "2") {
									tdArr.eq(parseInt(_id) + 1).find('select').empty();//错位（有序号）
									$.each(_list, function (i, item) {
										tdArr.eq(parseInt(_id) + 1).find('select').append(new Option(item.Text, item.Value));
									});
								}
							}
						}
					}
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
					if (flag.startsWith(_starts) && _tbid == 'tb_Subtable05') {
						tdArr.eq(parseInt(_v) + 1).find('input').val(_value);
					}
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
				if (flag.startsWith('#tb_Subtable05#') && flag.substr(-3) == '#0#') {
					var _row = { };
					_row.flag = flag;
					_row.v1 = tdArr.eq(2).find('input').val();//列12儿童
					_row.v2 = tdArr.eq(3).find('input').val();//列2
					_row.v3 = tdArr.eq(4).find('input').val();//列3
					_list.push(_row);
				}
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
				if (flag.startsWith('#tb_Subtable05#' + id + '#') && tbid =='tb_Subtable05') {
					_list.push({ ControlID: 'v1', ControlValue: tdArr.eq(2).find('input').val(), ControlType: '1' });//列12儿童
					_list.push({ ControlID: 'v2', ControlValue: tdArr.eq(3).find('input').val(), ControlType: '1' });//列2
					_list.push({ ControlID: 'v3', ControlValue: tdArr.eq(4).find('input').val(), ControlType: '1' });//列3
				}
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
				if (flag.startsWith('#tb_Subtable05#')) {
					var _row = { };
					_row.flag = flag;
					_row.v1 = tdArr.eq(2).find('input').val();//列12儿童
					_row.v2 = tdArr.eq(3).find('input').val();//列2
					_row.v3 = tdArr.eq(4).find('input').val();//列3
					_list.push(_row);
				}
			}
			return _list;
		},
		//设置子表不可编辑
		disabledGridTable: function (grid) {
			//控制子表工具栏按钮
			if (grid == 'tb_Subtable05') {
				$('#del_tb_Subtable05').attr('class', 'layui-btn layui-btn-disabled layui-btn-xs');
				$('#del_tb_Subtable05').removeAttr('lay-event');
				$('#add_tb_Subtable05').attr('class', 'layui-btn layui-btn-disabled layui-btn-xs');
				$('#add_tb_Subtable05').removeAttr('lay-event');
				$('#edit_tb_Subtable05').attr('class', 'layui-btn layui-btn-disabled layui-btn-xs');
				$('#edit_tb_Subtable05').removeAttr('lay-event');
				$('#add_tb_Subtable05').attr('disabled', 'true');
				$('#edit_tb_Subtable05').attr('disabled', 'true');
				$('#del_tb_Subtable05').attr('disabled', 'true');
			}
			var trList = $(".layui-table").find("tr");  //获取table下的所有tr
			for (var i = 0; i < trList.length; i++) {  //遍历所有的tr
				var tdArr = trList.eq(i).find("td"); //获取该tr下的所有td
				var flag = tdArr.eq(0).text();
				if (flag.startsWith('#tb_Subtable05#') && grid =='tb_Subtable05') {
					tdArr.eq(2).find('input').attr('disabled', 'disabled');//列12儿童
					tdArr.eq(3).find('input').attr('disabled', 'disabled');//列2
					tdArr.eq(4).find('input').attr('disabled', 'disabled');//列3
					tdArr.eq(5).find('button').attr('disabled', 'disabled');//右侧添加|删除按钮
					tdArr.eq(5).find('button').attr('class', 'layui-btn layui-btn-disabled layui-btn-xs');
					tdArr.eq(5).find('button').removeAttr('lay-event');
				}
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
	exports('AutomaticCalculation', api);
});