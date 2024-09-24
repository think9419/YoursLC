"use strict";
layui.define(["layer"], function (exports) {
    let exLayer = {
        /**
         * confirm()函数二次封装
         * @param content
         * @param yesFunction
         */
        confirm: function (content, yesFunction) {
            let options = { skin: exLayer.skinChoose(), icon: 3, title: "提示", anim: exLayer.animChoose() };
            layer.confirm(content, options, yesFunction);
        },
        /**
         * open()函数二次封装,支持在table页面和普通页面打开
         * @param title
         * @param content
         * @param width
         * @param height
         * @param successFunction
         * @param endFunction
         */
        open: function (title, content, width, height, top, left, successFunction, endFunction) {
            layer.open({
                title: title,
                type: 2,
                maxmin: true,
                shade: 0.5,
                /* anim: exLayer.animChoose(),*/
                area: [width, height],
                offset: [top, left],
                content: content,
                zIndex: layer.zIndex,
                /* skin: exLayer.skinChoose(),*/
                success: successFunction,
                end: endFunction
            });
        },
        openMiddle: function (title, content, width, height, ismobile) {
            if (ismobile) {
                layer.open({
                    title: title,
                    type: 2,
                    maxmin: true,
                    shade: 0.5,
                    /* anim: exLayer.animChoose(),*/
                    area: ['100%', height],
                    offset: 'auto',
                    content: content,
                    zIndex: layer.zIndex,
                    /* skin: exLayer.skinChoose(),*/
                    success: null,
                    end: null
                });
            }
            else {
                layer.open({
                    title: title,
                    type: 2,
                    maxmin: true,
                    shade: 0.5,
                    /* anim: exLayer.animChoose(),*/
                    area: [width, height],
                    offset: 'auto',
                    content: content,
                    zIndex: layer.zIndex,
                    /* skin: exLayer.skinChoose(),*/
                    success: null,
                    end: null
                });
            }
        },

        openMobile: function (title, content, width, height, top, left, successFunction, endFunction) {
            layer.open({
                title: title,
                type: 2,
                skin: 'layui-layer-setwin', //样式类名
                shade: 0.5,
                /* anim: exLayer.animChoose(),*/
                area: [width, height],
                offset: [top, left],
                content: content,
                zIndex: layer.zIndex,
                /* skin: exLayer.skinChoose(),*/
                success: successFunction,
                end: endFunction
            });
        },
        /**
         * msg()函数二次封装
         */
        // msg弹窗默认消失时间
        time: 2000,
        // 绿色勾
        greenTickMsg: function (content, callbackFunction) {
            let options = { icon: 1, time: exLayer.time, anim: exLayer.animChoose() };
            layer.msg(content, options, callbackFunction);
        },
        // 红色叉
        redCrossMsg: function (content, callbackFunction) {
            let options = { icon: 2, time: exLayer.time, anim: exLayer.animChoose() };
            layer.msg(content, options, callbackFunction);
        },
        // 黄色问号
        yellowQuestionMsg: function (content, callbackFunction) {
            let options = { icon: 3, time: exLayer.time, anim: exLayer.animChoose() };
            layer.msg(content, options, callbackFunction);
        },
        // 灰色锁
        grayLockMsg: function (content, callbackFunction) {
            let options = { icon: 4, time: exLayer.time, anim: exLayer.animChoose() };
            layer.msg(content, options, callbackFunction);
        },
        // 红色哭脸
        redCryMsg: function (content, callbackFunction) {
            let options = { icon: 5, time: exLayer.time, anim: exLayer.animChoose() };
            layer.msg(content, options, callbackFunction);
        },
        // 绿色笑脸
        greenLaughMsg: function (content, callbackFunction) {
            let options = { icon: 6, time: exLayer.time, anim: exLayer.animChoose() };
            layer.msg(content, options, callbackFunction);
        },
        // 黄色感叹号
        yellowSighMsg: function (content, callbackFunction) {
            let options = { icon: 7, time: exLayer.time, anim: exLayer.animChoose() };
            layer.msg(content, options, callbackFunction);
        },
        /**
         * 皮肤选择
         * @returns {string}
         */
        skinChoose: function () {
            //let storage = window.localStorage;
            //let skin = storage.getItem("skin");
            //if (skin == 1) {
            //    // 灰白色
            //    return "";
            //} else if (skin == 2) {
            //    // 墨绿色
            //    return "layui-layer-molv";
            //} else if (skin == 3) {
            //    // 蓝色
            //    return "layui-layer-lan";
            //} else if (!skin || skin == 4) {
            //    // 随机颜色
            //    var skinArray = ["", "layui-layer-molv", "layui-layer-lan"];
            //    return skinArray[Math.floor(Math.random() * skinArray.length)];
            //}

            return "";
        },
        /**
         * 动画选择
         * @returns {number}
         */
        animChoose: function () {
            //let storage = window.localStorage;
            //let anim = storage.getItem("anim");
            //let animArray = ["0", "1", "2", "3", "4", "5", "6"];
            //if (animArray.indexOf(anim) > -1) {
            //    // 用户选择的动画
            //    return anim;
            //} else if (!anim || anim == 7) {
            //    // 随机动画
            //    return Math.floor(Math.random() * animArray.length);
            //}

            return 0;
        }
    }

    exports("exLayer", exLayer);
});