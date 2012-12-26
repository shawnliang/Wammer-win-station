/**
 * Galleria Classic Theme 2012-08-08
 * http://galleria.io
 *
 * Licensed under the MIT license
 * https://raw.github.com/aino/galleria/master/LICENSE
 *
 */
(function(e){Galleria.addTheme({name:"classic",author:"Galleria",css:"galleria.classic.css",defaults:{transition:"slide",thumbCrop:"height",_toggleInfo:!0},init:function(t){Galleria.requires(1.28,"This version of Classic theme requires Galleria 1.2.8 or later"),this.addElement("info-link","info-close"),this.append({info:["info-link","info-close"]});var n=this.$("info-link,info-close,info-text"),r=Galleria.TOUCH,i=r?"touchstart":"click";this.$("loader,counter").show().css("opacity",.4),r||(this.addIdleState(this.get("image-nav-left"),{left:-50}),this.addIdleState(this.get("image-nav-right"),{right:-50}),this.addIdleState(this.get("counter"),{opacity:0})),t._toggleInfo===!0?n.bind(i,function(){n.toggle()}):(n.show(),this.$("info-link, info-close").hide()),this.bind("thumbnail",function(t){r?e(t.thumbTarget).css("opacity",this.getIndex()?1:.6):(e(t.thumbTarget).css("opacity",.6).parent().hover(function(){e(this).not(".active").children().stop().fadeTo(100,1)},function(){e(this).not(".active").children().stop().fadeTo(400,.6)}),t.index===this.getIndex()&&e(t.thumbTarget).css("opacity",1))}),this.bind("loadstart",function(t){t.cached||this.$("loader").show().fadeTo(200,.4),this.$("info").toggle(this.hasInfo()),e(t.thumbTarget).css("opacity",1).parent().siblings().children().css("opacity",.6)}),this.bind("loadfinish",function(e){this.$("loader").fadeOut(200)})}})})(jQuery);