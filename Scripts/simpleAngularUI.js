//Simple Angular UI
//Time Picker
//Authors: Amir Azizi
//Email: web1392@yahoo.com
//Version: 1.0.5

var app = angular.module("simpleAngularUI", []);
app.directive("suiTimePicker", function ($timeout) {

    return {
        restrict: "E",
        replace: true,
        scope: {
            time: "=",
            stringTime: "=",
            hasSecond: "=",
            ngDisabled: "=",
            ngRequired: "=",
            maxHour: "@",
            size: "@",
            dirType: "@",
            validation: "&"
        },
        template: '<div class="input-group" ng-class="templateClass">' +
                     '<input type="text" class="form-control text-center" ng-class="inputClassHour" name="hour" maxlength="2" ng-model="timespan.h" ng-keydown="onHourChange($event)" ng-blur="convertToTime()" autocomplete="off" ng-disabled="ngDisabled" ng-required="ngRequired" placeholder="ساعت">' +
                     '<span class="input-group-addon addon-no-border"><b>:</b></span>' +
                     '<input type="text" class="form-control text-center pull-left" ng-class="inputClassMinute" name="minute" maxlength="2" ng-model="timespan.m" ng-keydown="onMinuteChange($event)" ng-blur="convertToTime()" autocomplete="off" ng-disabled="ngDisabled" ng-required="ngRequired" placeholder="دقیقه">' +
                     '<span class="input-group-addon addon-no-border" ng-if="hasSecond"><b>:</b></span>' +
                     '<input type="text" class="form-control text-center pull-left" ng-class="inputClassSecond" name="second" maxlength="2" ng-if="hasSecond" ng-model="timespan.s" ng-keydown="onSecondChange($event)" ng-blur="convertToTime()" autocomplete="off" ng-disabled="ngDisabled" ng-required="ngRequired" placeholder="ثانیه">' +
                "</div>",

        link: function (scope, element, ngModel) {

            $timeout(function () {
                if (angular.isUndefined(scope.ngRequired)) {
                    console.error("please add ng-required to your input/textarea/select");
                    return false;
                } else {
                    if (scope.ngRequired) {
                        if (scope.timespan.h == "undefined" || angular.isUndefined(scope.timespan.h)) scope.inputClassHour = 'required-alert';
                        if (scope.timespan.m == "undefined" || angular.isUndefined(scope.timespan.m)) scope.inputClassMinute = 'required-alert';
                        if (scope.timespan.s == "undefined" || angular.isUndefined(scope.timespan.s)) scope.inputClassSecond = 'required-alert';
                    }
                    element.find("input[name='hour']").bind("blur", function () {
                        if (scope.timespan.h == "undefined" || angular.isUndefined(scope.timespan.h) || scope.timespan.h.length == 0) {
                            scope.timespan.h = "";
                            scope.inputClassHour = 'required-alert';
                        } else {
                            scope.inputClassHour = 'required-alert-ok';
                        }
                    });
                    element.find("input[name='minute']").bind("blur", function () {
                        if (scope.timespan.m == "undefined" || angular.isUndefined(scope.timespan.m) || scope.timespan.m.length==0) {
                            scope.timespan.m = "";
                            scope.inputClassMinute = 'required-alert';
                        } else {
                            scope.inputClassMinute = 'required-alert-ok';
                        }
                    });
                    element.find("input[name='second']").bind("blur", function () {
                        if (scope.timespan.s == "undefined" || angular.isUndefined(scope.timespan.s) || scope.timespan.s.length == 0) {
                            scope.timespan.s = "";
                            scope.inputClassSecond = 'required-alert';
                        } else {
                            scope.inputClassSecond = 'required-alert-ok';
                        }
                    });

                }
                return false;
            }, 1500)




            scope.timespan = { h: "", m: "", s: "" };
            //set css class 
            scope.templateClass = "";
            if (scope.dirType === "rtl") {
                scope.templateClass = "rtl-timepicker";
                switch (scope.size) {
                    case "lg":
                        scope.templateClass = scope.templateClass + " input-group-lg rtl-timepicker-lg";
                        break;
                    case "sm":
                        scope.templateClass = scope.templateClass + " input-group-sm rtl-timepicker-sm";
                        break;
                }
            } else {
                switch (scope.size) {
                    case "lg":
                        scope.templateClass = "input-group-lg";
                        break;
                    case "sm":
                        scope.templateClass = "input-group-sm";
                        break;
                }
            }

            //Set the default value for hasSecond
            if (angular.isUndefined(scope.hasSecond)) {
                scope.hasSecond = true;
            }

            //Set the default value for hasSecond
            if (angular.isUndefined(scope.maxHour)) {
                scope.maxHour = 24;
            }

            //Converting string Time to Object Time
            scope.timeAnalyzer = function (time) {
                var hms = time.split(":");
                var h, m, s;
                if (scope.hasSecond) {
                    h = hms[0];
                    m = hms[1];
                    s = hms[2];
                    return { h: h, m: m, s: s };
                } else {
                    h = hms[0];
                    m = hms[1];
                    return { h: h, m: m };
                }
            }

            //Converting Object Time to string Time
            scope.convertToTime = function () {
                if (scope.stringTime) {
                    //Correction hour
                    if (scope.timespan.h.length === 1) {
                        scope.timespan.h = "0" + scope.timespan.h;
                    }
                    //Correction min
                    if (scope.timespan.m.length === 1) {
                        scope.timespan.m = "0" + scope.timespan.m;
                    }
                    //Check hasSecond
                    if (scope.hasSecond) {
                        //Correction Second
                        if (scope.timespan.s.length === 1) {
                            scope.timespan.s = "0" + scope.timespan.s;
                        }
                        scope.time = scope.timespan.h + ":" + scope.timespan.m + ":" + scope.timespan.s;
                    } else {
                        scope.time = scope.timespan.h + ":" + scope.timespan.m;
                    }
                    scope.validation();
                }
            }

            //Check String Time
            if (scope.stringTime) {
                //watch time variable
                scope.$watch("time", function (newValue) {
                    if (newValue) {
                        if (!angular.isUndefined(scope.time)) {
                            scope.timespan = scope.timeAnalyzer(scope.time);
                        }
                    }
                }, true);

                //watch timespan variable
                scope.$watch("timespan", function (newValue) {
                    if (scope.hasSecond) {
                        scope.time = newValue.h + ":" + newValue.m + ":" + newValue.s;
                    } else {
                        scope.time = newValue.h + ":" + newValue.m;
                    }
                }, true);

            } else {
                //directive check config
                if (angular.isUndefined(scope.time.h)) {
                    console.error("please return object {h:?,m:?,s:?}");
                }

                //watch time variable
                scope.$watch("time", function (newValue) {
                    if (newValue) {
                        if (!angular.isUndefined(scope.time)) {
                            scope.timespan = scope.time;
                        }
                    }
                }, true);
            }

            //Select All Text When Click or Focus on input
            element.find("input[type='text']").each(function () {
                $(this).on("click", function () {
                    $(this).select();
                });
            });

            //check the value of number
            element.find("input[name='hour']").on("input propertychange", function () {
                var val = $(this).val();
                val = val.replace(/[a-zA-Z?><;,\\"`~»«×÷\.:{}()[\]\-_+=!@/#$%\^&*|']*/g, "");
                val = val.replace(/[\u0600-\u06FF ]/g, "");
                if (val > scope.maxHour - 1) {
                    val = "00";
                    scope.timespan.h = 0;
                }

                $(this).val(val);
            });

            element.find("input[name='minute'], input[name='second']").on("input propertychange", function () {
                var val = $(this).val();
                val = val.replace(/[a-zA-Z?><;,\\"`~»«×÷\.:{}()[\]\-_+=!@/#$%\^&*|']*/g, "");
                val = val.replace(/[\u0600-\u06FF ]/g, "");
                if (val > 59) {
                    val = "00";
                    scope.timespan.m = 0;
                }
                $(this).val(val);
            });

            // Respond on up/down/left/right arrowkeys
            var getKeyboardEventResult = function (keyEvent) {
                return window.event ? keyEvent.keyCode : keyEvent.which;
            };

            scope.onHourChange = function ($event) {
                var keyCode = getKeyboardEventResult($event);
                if (keyCode === 38) {//up
                    if (scope.timespan.h < scope.maxHour - 1) {
                        scope.timespan.h++;
                    }
                    else {
                        scope.timespan.h = 0;
                    }
                }
                if (keyCode === 40) {//down
                    if (scope.timespan.h > 0) {
                        scope.timespan.h--;
                    } else {
                        scope.timespan.h = scope.maxHour - 1;
                    }
                }
                if (keyCode === 39) {
                    setTimeout(function () {//right
                        $(element).find("input[name='minute']").select();
                        scope.$apply();
                    }, 0);
                }
            };

            scope.onMinuteChange = function ($event) {
                var keyCode = getKeyboardEventResult($event);
                if (keyCode === 38) {//up
                    if (scope.timespan.m < 59) {
                        scope.timespan.m++;
                    } else {
                        scope.timespan.m = 0;
                        if (scope.timespan.h < scope.maxHour - 1) {
                            scope.timespan.h++;
                        } else {
                            scope.timespan.h = 0;
                        }

                    }
                }
                if (keyCode === 40) {//down

                    if (scope.timespan.m > 0) {
                        scope.timespan.m--;
                    } else {
                        scope.timespan.m = 59;
                        if (scope.timespan.h > 0) {
                            scope.timespan.h--;
                        } else {
                            scope.timespan.h = scope.maxHour - 1;
                        }
                    }
                }
                if (keyCode === 39) {//right
                    setTimeout(function () {
                        $(element).find("input[name='second']").select();
                        scope.$apply();
                    }, 0);
                }
                if (keyCode === 37) {
                    setTimeout(function () {//left
                        $(element).find("input[name='hour']").select();
                        scope.$apply();
                    }, 0);
                }
            };

            scope.onSecondChange = function ($event) {
                var keyCode = getKeyboardEventResult($event);
                if (keyCode === 38) {//up
                    if (scope.timespan.s < 59) {
                        scope.timespan.s++;
                    } else {
                        scope.timespan.s = 0;
                        if (scope.timespan.m < 59) {
                            scope.timespan.m++;
                        } else {
                            scope.timespan.m = 0;
                            if (scope.timespan.h < scope.maxHour - 1) {
                                scope.timespan.h++;
                            } else {
                                scope.timespan.h = 0;
                            }
                        }
                    }
                }
                if (keyCode === 40) {//down
                    if (scope.timespan.s > 0) {
                        scope.timespan.s--;
                    } else {
                        scope.timespan.s = 59;
                        if (scope.timespan.m > 0) {
                            scope.timespan.m--;
                        } else {
                            scope.timespan.m = 59;
                            if (scope.timespan.h > 0) {
                                scope.timespan.h--;
                            } else {
                                scope.timespan.h = scope.maxHour - 1;
                            }
                        }
                    }
                }
                if (keyCode === 37) {//left
                    setTimeout(function () {
                        $(element).find("input[name='minute']").select();
                        scope.$apply();
                    }, 0);
                }
            };
        }
    };
});