/**
 * Galleria History Plugin 2012-04-04
 * http://galleria.io
 *
 * Licensed under the MIT license
 * https://raw.github.com/aino/galleria/master/LICENSE
 *
 */
(function(e,t){Galleria.requires(1.25,"The History Plugin requires Galleria version 1.2.5 or later."),Galleria.History=function(){var n=[],r=!1,i=t.location,s=t.document,o=Galleria.IE,u="onhashchange"in t&&(s.mode===undefined||s.mode>7),a,f=function(e){return a&&!u&&Galleria.IE?e=e||a.location:e=i,parseInt(e.hash.substr(2),10)},l=f(i),c=[],h=function(){e.each(c,function(e,n){n.call(t,f())})},p=function(){e.each(n,function(e,t){t()}),r=!0},d=function(e){return"/"+e};return u&&o<8&&(u=!1),u?p():e(function(){var n=t.setInterval(function(){var e=f();!isNaN(e)&&e!=l&&(l=e,i.hash=d(e),h())},50);o?e('<iframe tabindex="-1" title="empty">').hide().attr("src","about:blank").one("load",function(){a=this.contentWindow,p()}).insertAfter(s.body):p()}),{change:function(e){c.push(e),u&&(t.onhashchange=h)},set:function(e){if(isNaN(e))return;!u&&o&&this.ready(function(){var t=a.document;t.open(),t.close(),a.location.hash=d(e)}),i.hash=d(e)},ready:function(e){r?e():n.push(e)}}}()})(jQuery,this);