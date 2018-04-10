/*!CK:1705559581!*//*1447048399,*/

if (self.CavalryLogger) { CavalryLogger.start_js(["SVsQw"]); }

__d('padNumber',[],function a(b,c,d,e,f,g){'use strict';if(c.__markCompiled)c.__markCompiled();function h(i,j){var k=i.toString();if(k.length>=j)return k;return Array(j-k.length+1).join('0')+k;}f.exports=h;},null);
__d('PureStoreBasedStateMixin',['StoreBasedStateMixinHelper','invariant','setImmediate'],function a(b,c,d,e,f,g,h,i,j){'use strict';if(c.__markCompiled)c.__markCompiled();var k=(function(){for(var l=arguments.length,m=Array(l),n=0;n<l;n++)m[n]=arguments[n];return {getInitialState:function(){return this.constructor.calculateState();},componentWillMount:function(){!this.constructor.calculateState?i(0):undefined;this._recalculateStateID=null;var o=(function(){if(this.isMounted())this.setState(this.constructor.calculateState());this._recalculateStateID=null;}).bind(this);this._mixin=new h(m);this._mixin.subscribeCallback((function(){if(this._recalculateStateID===null)this._recalculateStateID=j(o);}).bind(this));},componentWillUnmount:function(){this._mixin.release();this._mixin=null;}};}).bind(this);f.exports=k;},null);
__d('DateStrings',['fbt'],function a(b,c,d,e,f,g,h){if(c.__markCompiled)c.__markCompiled();var i,j,k,l,m,n,o,p,q={getWeekdayName:function(r){if(!n)n=[h._("Sunday"),h._("Monday"),h._("Tuesday"),h._("Wednesday"),h._("Thursday"),h._("Friday"),h._("Saturday")];return n[r];},getUppercaseWeekdayName:function(r){if(!p)p=[h._("SUNDAY"),h._("MONDAY"),h._("TUESDAY"),h._("WEDNESDAY"),h._("THURSDAY"),h._("FRIDAY"),h._("SATURDAY")];return p[r];},getWeekdayNameShort:function(r){if(!o)o=[h._("Sun"),h._("Mon"),h._("Tue"),h._("Wed"),h._("Thu"),h._("Fri"),h._("Sat")];return o[r];},getMonthName:function(r){if(!i)i=[h._("January"),h._("February"),h._("March"),h._("April"),h._("May"),h._("June"),h._("July"),h._("August"),h._("September"),h._("October"),h._("November"),h._("December")];return i[r-1];},getUppercaseMonthName:function(r){if(!l)l=[h._("JANUARY"),h._("FEBRUARY"),h._("MARCH"),h._("APRIL"),h._("MAY"),h._("JUNE"),h._("JULY"),h._("AUGUST"),h._("SEPTEMBER"),h._("OCTOBER"),h._("NOVEMBER"),h._("DECEMBER")];return l[r-1];},getMonthNameShort:function(r){if(!j)j=[h._("Jan"),h._("Feb"),h._("Mar"),h._("Apr"),h._("May"),h._("Jun"),h._("Jul"),h._("Aug"),h._("Sep"),h._("Oct"),h._("Nov"),h._("Dec")];return j[r-1];},getUppercaseMonthNameShort:function(r){if(!k)k=[h._("JAN"),h._("FEB"),h._("MAR"),h._("APR"),h._("MAY"),h._("JUN"),h._("JUL"),h._("AUG"),h._("SEP"),h._("OCT"),h._("NOV"),h._("DEC")];return k[r-1];},getOrdinalSuffix:function(r){if(!m)m=['',h._("st"),h._("nd"),h._("rd"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("st"),h._("nd"),h._("rd"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("th"),h._("st")];return m[r];},getDayLabel:function(){return h._("Day:");},getMonthLabel:function(){return h._("Month:");},getYearLabel:function(){return h._("Year:");},getDayPlaceholder:function(){return h._("dd");},getMonthPlaceholder:function(){return h._("mm");},getYearPlaceholder:function(){return h._("yyyy");},get12HourClockSuffix:function(r){if(r<12)return h._("am");return h._("pm");},getUppercase12HourClockSuffix:function(r){if(r<12)return h._("AM");return h._("PM");}};f.exports=q;},null);
__d('formatDate',['DateStrings','DateFormatConfig','fbt','invariant','padNumber'],function a(b,c,d,e,f,g,h,i,j,k,l){if(c.__markCompiled)c.__markCompiled();function m(p,q,r){r=r||{};if(!q||!p)return '';if(typeof p==='string')p=parseInt(p,10);if(typeof p==='number')p=new Date(p*1000);!(p instanceof Date)?k(0):undefined;!!isNaN(p.getTime())?k(0):undefined;!(p.getTime()<1e+15)?k(0):undefined;if(typeof q!=='string'){var s=n();for(var t in s){var u=s[t];if(u.start<=p.getTime()&&q[u.name]){q=q[u.name];break;}}}var v;if(r.skipPatternLocalization||!r.formatInternal&&o()||q.length===1){v=q;}else{!i.formats[q]?k(0):undefined;v=i.formats[q];}var w=r.utc?'getUTC':'get',x=p[w+'Date'](),y=p[w+'Day'](),z=p[w+'Month'](),aa=p[w+'FullYear'](),ba=p[w+'Hours'](),ca=p[w+'Minutes'](),da=p[w+'Seconds'](),ea=p[w+'Milliseconds'](),fa='';for(var ga=0;ga<v.length;ga++){var ha=v.charAt(ga);switch(ha){case '\\':ga++;fa+=v.charAt(ga);break;case 'd':fa+=l(x,2);break;case 'j':fa+=x;break;case 'S':fa+=h.getOrdinalSuffix(x);break;case 'D':fa+=h.getWeekdayNameShort(y);break;case 'l':fa+=h.getWeekdayName(y);break;case 'F':case 'f':fa+=h.getMonthName(z+1);break;case 'M':fa+=h.getMonthNameShort(z+1);break;case 'm':fa+=l(z+1,2);break;case 'n':fa+=z+1;break;case 'Y':fa+=aa;break;case 'y':fa+=(''+aa).slice(2);break;case 'a':fa+=h.get12HourClockSuffix(ba);break;case 'A':fa+=h.getUppercase12HourClockSuffix(ba);break;case 'g':fa+=ba===0||ba===12?12:ba%12;break;case 'G':fa+=ba;break;case 'h':if(ba===0||ba===12){fa+=12;}else fa+=l(ba%12,2);break;case 'H':fa+=l(ba,2);break;case 'i':fa+=l(ca,2);break;case 's':fa+=l(da,2);break;case 'X':fa+=l(ea,3);break;default:fa+=ha;}}return fa;}function n(){var p=new Date(),q=p.getTime(),r=p.getFullYear(),s=p.getDate()-(p.getDay()-i.weekStart+6)%7,t=new Date(r,p.getMonth()+1,0).getDate(),u=new Date(r,1,29).getMonth()===1?366:365,v=1000*60*60*24;return [{name:'today',start:p.setHours(0,0,0,0)},{name:'withinDay',start:q-v},{name:'thisWeek',start:new Date(p.getTime()).setDate(s)},{name:'withinWeek',start:q-v*7},{name:'thisMonth',start:p.setDate(1)},{name:'withinMonth',start:q-v*t},{name:'thisYear',start:p.setMonth(0)},{name:'withinYear',start:q-v*u},{name:'older',start:-Infinity}];}m.periodNames=['today','thisWeek','thisMonth','thisYear','withinDay','withinWeek','withinMonth','withinYear','older'];function o(){if(typeof window==='undefined'||!window||!window.location||!window.location.pathname)return false;var p=window.location.pathname,q='/intern';return p.substr(0,q.length)===q;}f.exports=m;},null);
__d('keyMirrorRecursive',['invariant'],function a(b,c,d,e,f,g,h){'use strict';if(c.__markCompiled)c.__markCompiled();function i(l,m){return j(l,m);}function j(l,m){var n={},o;!k(l)?h(0):undefined;for(o in l){if(!l.hasOwnProperty(o))continue;var p=l[o],q=m?m+'.'+o:o;if(k(p)){p=j(p,q);}else p=q;n[o]=p;}return n;}function k(l){return l instanceof Object&&!Array.isArray(l);}f.exports=i;},null);
__d('mergeDeepInto',['invariant','mergeHelpers'],function a(b,c,d,e,f,g,h,i){'use strict';if(c.__markCompiled)c.__markCompiled();var j=i.ArrayStrategies,k=i.checkArrayStrategy,l=i.checkMergeArrayArgs,m=i.checkMergeLevel,n=i.checkMergeObjectArgs,o=i.isTerminal,p=i.normalizeMergeArg,q=function(u,v,w,x){n(u,v);m(x);var y=v?Object.keys(v):[];for(var z=0;z<y.length;z++){var aa=y[z];s(u,v,aa,w,x);}},r=function(u,v,w,x){l(u,v);m(x);var y=Math.max(u.length,v.length);for(var z=0;z<y;z++)s(u,v,z,w,x);},s=function(u,v,w,x,y){var z=v[w],aa=v.hasOwnProperty(w),ba=aa&&o(z),ca=aa&&Array.isArray(z),da=aa&&!ca&&!ca,ea=u[w],fa=u.hasOwnProperty(w),ga=fa&&o(ea),ha=fa&&Array.isArray(ea),ia=fa&&!ha&&!ha;if(ga){if(ba){u[w]=z;}else if(ca){u[w]=[];r(u[w],z,x,y+1);}else if(da){u[w]={};q(u[w],z,x,y+1);}else if(!aa)u[w]=ea;}else if(ha){if(ba){u[w]=z;}else if(ca){!j[x]?h(0):undefined;if(x===j.Clobber)ea.length=0;r(ea,z,x,y+1);}else if(da){u[w]={};q(u[w],z,x,y+1);}else !aa;}else if(ia){if(ba){u[w]=z;}else if(ca){u[w]=[];r(u[w],z,x,y+1);}else if(da){q(ea,z,x,y+1);}else !aa;}else if(!fa)if(ba){u[w]=z;}else if(ca){u[w]=[];r(u[w],z,x,y+1);}else if(da){u[w]={};q(u[w],z,x,y+1);}else !aa;},t=function(u,v,w){var x=p(v);k(w);q(u,x,w,0);};f.exports=t;},null);
__d('mergeDeep',['mergeHelpers','mergeDeepInto'],function a(b,c,d,e,f,g,h,i){'use strict';if(c.__markCompiled)c.__markCompiled();var j=h.checkArrayStrategy,k=h.checkMergeObjectArgs,l=h.normalizeMergeArg,m=function(n,o,p){var q=l(n),r=l(o);k(q,r);j(p);var s={};i(s,q,p);i(s,r,p);return s;};f.exports=m;},null);
__d('ReactComponentRenderer',['React','ReactDOM','Object.assign','warning'],function a(b,c,d,e,f,g,h,i,j,k){'use strict';if(c.__markCompiled)c.__markCompiled();function l(m,n){this.klass=m;this.container=n;this.props={};this.component=null;}l.prototype.replaceProps=function(m,n){this.props={};this.setProps(m,n);};l.prototype.setProps=function(m,n){if(this.klass==null)return;j(this.props,m);var o=h.createElement(this.klass,this.props);this.component=i.render(o,this.container,n);};l.prototype.unmount=function(){i.unmountComponentAtNode(this.container);this.klass=null;};f.exports=l;},null);
__d('DropdownContextualHelpLink',['DOM','ge'],function a(b,c,d,e,f,g,h,i){if(c.__markCompiled)c.__markCompiled();var j={set:function(k){var l=i('navHelpCenter');if(l!==null)h.replace(l,k);}};f.exports=j;},null);
__d('WebMessengerEvents',['Arbiter'],function a(b,c,d,e,f,g,h){if(c.__markCompiled)c.__markCompiled();var i=Object.assign(new h(),{MASTER_DOM_CHANGED:'master-dom-changed',DETAIL_DOM_CHANGED:'detail-dom-changed',FOCUS_COMPOSER:'focus-composer',FOCUS_SEARCH:'focus-search',FOCUS_AND_SELECT_SEARCH:'focus-and-select-search',STICKER_CLICKED:'sticker-clicked',SUBMIT_REPLY:'submit-reply',UPDATE_SELECTION:'update-selection',masterDOMChanged:function(){this.inform(i.MASTER_DOM_CHANGED);},detailDOMChanged:function(){this.inform(i.DETAIL_DOM_CHANGED);},focusComposer:function(){this.inform(i.FOCUS_COMPOSER);},focusSearch:function(){this.inform(i.FOCUS_SEARCH);},focusAndSelectSearch:function(){this.inform(i.FOCUS_AND_SELECT_SEARCH);},updateSelection:function(j){this.inform(i.UPDATE_SELECTION,j);},stickerClicked:function(j,k){this.inform(i.STICKER_CLICKED,{packID:j,stickerID:k});},submitReply:function(){this.inform(i.SUBMIT_REPLY);}});f.exports=i;},null);
__d('WebMessengerSubscriptionsHandler',['SubscriptionsHandler'],function a(b,c,d,e,f,g,h){if(c.__markCompiled)c.__markCompiled();var i=new h('webmessenger');f.exports=i;},null);
__d("isWebMessengerURI",[],function a(b,c,d,e,f,g){if(c.__markCompiled)c.__markCompiled();function h(i){return (/^(\/messages)/.test(i.getPath()));}f.exports=h;},null);
__d('WebMessengerWidthControl',['Arbiter','BlueBar','CSS','CSSClassTransition','DOMDimensions','Event','Style','URI','ViewportBounds','WebMessengerEvents','WebMessengerSubscriptionsHandler','$','cx','isWebMessengerURI','requestAnimationFrame','setTimeoutAcrossTransitions','shield','throttle'],function a(b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y){if(c.__markCompiled)c.__markCompiled();var z=205,aa=981,ba=257,ca=18,da=848,ea=724,fa=.57,ga=56,ha,ia,ja;function ka(pa,qa,ra){this.masterChanged=pa;this.detailChaned=qa;r.addSubscriptions(m.listen(window,'resize',y(x(la,this,this),100)),h.subscribe(['sidebar/initialized','sidebar/show','sidebar/hide','minisidebar/show'],x(la,this,this),h.SUBSCRIBE_NEW));var sa=oa()?ga:0;if(ra)sa=z;this._width=oa()?0:da;ja=true;la(this,sa);}function la(pa,qa){var ra=p.getRight()+p.getLeft();ra=ra||qa||0;var sa=l.getViewportWithoutScrollbarDimensions().width-ra,ta=Math.round(Math.max(0,sa/2-aa/2));sa=aa+ta-ba;sa-=ca;sa=Math.max(ea,Math.min(da,sa));if(!isNaN(sa)&&pa._width!==sa){pa._width=sa;var ua=Math.round(sa/(1+fa)),va=sa-ua;pa.masterChanged(va);pa.detailChaned(ua);if(oa()){var wa=sa+ba;ma(function(){if(ia){document.body.className=ia;ia='';}na(wa+'px');j.removeClass(document.body,"_5uj5");ja&&q.detailDOMChanged();ja=false;},ia);}}}function ma(pa,qa){qa&&j.addClass(document.documentElement,"_5uj6");v(pa);qa&&w(j.removeClass.bind(null,document.documentElement,"_5uj6"),1000);}function na(pa){n.set(i.getNavRoot(),'width',pa);n.set(s('globalContainer'),'width',pa);}function oa(){if(!ha)ha=j.hasClass(document.body,"_6nw");return ha;}k.registerHandler(function(pa,qa,ra,sa){function ta(ua){return oa()&&u(new o(ua));}if(ta(sa)){ia=qa;return true;}else if(ta(ra)){ma(function(){pa.className=qa;na('');},true);return true;}});f.exports=ka;},null);
__d('SoundPlayer',['Arbiter','Flash','URI','createArrayFromMixed'],function a(b,c,d,e,f,g,h,i,j,k){if(c.__markCompiled)c.__markCompiled();var l={},m=null,n=false,o='so_sound_player',p='/swf/SoundPlayer.swf?v=1',q=null,r={};function s(ca){var da=new j(ca);if(!da.getDomain())return new j(window.location.href).setPath(da.getPath()).toString();return ca;}function t(ca){var da=new j(ca).getPath();if(/\.mp3$/.test(da))return 'audio/mpeg';if(/\.og[ga]$/.test(da))return 'audio/ogg';return '';}function u(){if(!q){var ca=document.createElement('audio');if(!ca||!ca.canPlayType)return null;ca.setAttribute('preload','auto');document.body.appendChild(ca);q=ca;}return q;}function v(ca){return r[ca];}function w(ca,da){r[ca]=da;}function x(){var ca=document[o]||window[o];if(ca)if(!ca.playSound&&ca.length)ca=ca[0];return ca&&ca.playSound&&ca.loopSound?ca:null;}function y(){return n;}function z(ca,da,ea){m={path:ca,sync:da,loop:ea};}function aa(){n=true;if(m){var ca=x();if(m.loop){ca.loopSound(m.path,m.sync);}else ca.playSound(m.path,m.sync);}}var ba={init:function(ca){ca=k(ca);var da;for(var ea=0;ea<ca.length;++ea){da=ca[ea];if(l[da])return;}var fa=u();for(ea=0;fa&&ea<ca.length;++ea){da=ca[ea];if(fa.canPlayType(da)){l[da]=true;return;}}l['audio/mpeg']=true;if(x())return;try{h.registerCallback(aa,['sound/player_ready','sound/ready']);var ha=document.createElement('div');ha.id='sound_player_holder';document.body.appendChild(ha);var ia=window[o]=i.embed(p,ha,{allowscriptaccess:'always',wmode:'transparent'},{swf_id:o});ia.setAttribute('id',o);ia.setAttribute('width','1px');ia.setAttribute('height','1px');h.inform('sound/player_ready');}catch(ga){}},createAndPlayNewNative:function(ca,da){var ea=document.createElement('audio');ea.setAttribute('preload','auto');document.body.appendChild(ea);ea.src=s(ca);if(da){ea.setAttribute('loop','');}else ea.removeAttribute('loop');ea.play();w(ca,ea);return;},play:function(ca,da){ca=k(ca);var ea=u(),fa,ga;for(var ha=0;ea&&ha<ca.length;++ha){fa=ca[ha];var ia=v(fa);if(ia){if(da){ia.setAttribute('loop','');}else ia.removeAttribute('loop');ia.play();return;}ga=t(fa);if(!ea.canPlayType(ga))continue;ba.init([ga]);ba.createAndPlayNewNative(fa,da);return;}for(ha=0;ha<ca.length;++ha){fa=s(ca[ha]);ga=t(fa);if(ga!='audio/mpeg')continue;ba.init([ga]);var ja=x();if(!y()){z(fa,true,da);return;}else if(ja){if(da){ja.loopSound(fa,true);}else ja.playSound(fa,true);return;}}},stop:function(ca){ca=k(ca);for(var da=0;da<ca.length;++da){var ea=s(ca[da]),fa=v(ca[da]),ga=x();if(fa&&fa.src==ea){fa.pause();fa.removeAttribute('src');fa.src=ea;}else ga&&ga.stopSound(ea);}}};f.exports=ba;},null);
__d('SoundSynchronizer',['SoundPlayer','WebStorage','createArrayFromMixed'],function a(b,c,d,e,f,g,h,i,j){if(c.__markCompiled)c.__markCompiled();var k='fb_sounds_playing3';function l(){var p=i.getLocalStorage();if(p)try{var r=p[k];if(r){r=JSON.parse(r);if(Array.isArray(r))return r;}}catch(q){}return [];}function m(p){var q=i.getLocalStorage();if(q){var r=l();r.push(p);while(r.length>5)r.shift();try{q[k]=JSON.stringify(r);}catch(s){}}}function n(p){return l().some(function(q){return q===p;});}var o={play:function(p,q,r){p=j(p);q=q||p[0]+Math.floor(Date.now()/1000);if(n(q))return;h.play(p,r);m(q);},isSupported:function(){return !!i.getLocalStorage();}};f.exports=o;},null);
__d('SoundRPC',['Event','SoundSynchronizer'],function a(b,c,d,e,f,g,h,i){if(c.__markCompiled)c.__markCompiled();function j(l,m,n){i.play(l,m,n);}var k={playLocal:j,playRemote:function(l,m,n,o){var p={paths:m,sync:n,loop:o};l.postMessage(JSON.stringify(p),'*');},supportsRPC:function(){return !!window.postMessage;},_listen:function(){h.listen(window,'message',function(l){if(!/\.facebook.com$/.test(l.origin))return;var m=JSON.parse(l.data||'{}');j(m.paths,m.sync,m.loop);});}};f.exports=k;},null);