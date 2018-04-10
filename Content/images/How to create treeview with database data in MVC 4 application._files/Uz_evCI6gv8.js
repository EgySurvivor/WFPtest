/*!CK:936808115!*//*1439192639,*/

if (self.CavalryLogger) { CavalryLogger.start_js(["I6t2K"]); }

__d("HelloCDN-loader",[],function a(b,c,d,e,f,g){if(c.__markCompiled)c.__markCompiled();function h(i){try{var k=document.createElement("IFRAME");k.src=i.testing_url;k.style.position="absolute";k.style.left="-1000px";k.style.top="-1000px";k.style.height="1px";k.style.width="1px";k.onload=function(){k.contentWindow.postMessage(JSON.stringify({config:i.cases}),"*");};document.body.appendChild(k);}catch(j){}}g.run=function(i){window.setTimeout(function(){i.resources.forEach(function(j){h(j);});},0,false);};},null);