(function(){function d(a){return function(b){this[a]=b}}function f(a){return function(){return this[a]}}var h;
function i(a,b,c){this.extend(i,google.maps.OverlayView);this.a=a;this.b=[];this.m=[];this.X=[53,56,66,78,90];this.j=[];this.v=false;c=c||{};this.f=c.gridSize||60;this.T=c.maxZoom||null;this.j=c.styles||[];this.Q=c.imagePath||this.J;this.P=c.imageExtension||this.I;this.Y=c.zoomOnClick||true;this.S=c.infoOnClick||false;this.R=c.infoOnClickZoom||0;l(this);this.setMap(a);this.D=this.a.getZoom();var e=this;google.maps.event.addListener(this.a,"zoom_changed",function(){var g=e.a.mapTypes[e.a.getMapTypeId()].maxZoom,
k=e.a.getZoom();if(!(k<0||k>g))if(e.D!=k){e.D=e.a.getZoom();e.n()}});google.maps.event.addListener(this.a,"bounds_changed",function(){e.i()});b&&b.length&&this.z(b,false)}h=i.prototype;h.J="http://google-maps-utility-library-v3.googlecode.com/svn/trunk/markerclusterer/images/m";h.I="png";h.extend=function(a,b){return function(c){for(property in c.prototype)this.prototype[property]=c.prototype[property];return this}.apply(a,[b])};h.onAdd=function(){if(!this.v){this.v=true;m(this)}};h.O=function(){};
h.draw=function(){};function l(a){for(var b=0,c;c=a.X[b];b++)a.j.push({url:a.Q+(b+1)+"."+a.P,height:c,width:c})}h.u=f("j");h.L=f("b");h.N=function(){return this.b.length};h.C=function(){return this.T||this.a.mapTypes[this.a.getMapTypeId()].maxZoom};h.A=function(a,b){for(var c=0,e=a.length,g=e;g!==0;){g=parseInt(g/10,10);c++}c=Math.min(c,b);return{text:e,index:c}};h.V=d("A");h.B=f("A");h.z=function(a,b){for(var c=0,e;e=a[c];c++)n(this,e);b||this.i()};
function n(a,b){b.setVisible(false);b.setMap(null);b.q=false;b.draggable&&google.maps.event.addListener(b,"dragend",function(){b.q=false;a.n();a.i()});a.b.push(b)}h.p=function(a,b){n(this,a);b||this.i()};h.U=function(a){var b=-1;if(this.b.indexOf)b=this.b.indexOf(a);else for(var c=0,e;e=this.b[c];c++)if(e==a)b=c;if(b==-1)return false;this.b.splice(b,1);a.setVisible(false);a.setMap(null);this.n();this.i();return true};h.M=function(){return this.m.length};h.getMap=f("a");h.setMap=d("a");h.t=f("f");
h.W=d("f");function o(a,b){var c=a.getProjection(),e=new google.maps.LatLng(b.getNorthEast().lat(),b.getNorthEast().lng()),g=new google.maps.LatLng(b.getSouthWest().lat(),b.getSouthWest().lng());e=c.fromLatLngToDivPixel(e);e.x+=a.f;e.y-=a.f;g=c.fromLatLngToDivPixel(g);g.x-=a.f;g.y+=a.f;e=c.fromDivPixelToLatLng(e);c=c.fromDivPixelToLatLng(g);b.extend(e);b.extend(c);return b}h.K=function(){this.n();this.b=[]};
h.n=function(){for(var a=0,b;b=this.m[a];a++)b.remove();for(a=0;b=this.b[a];a++){b.q=false;b.setMap(null);b.setVisible(false)}this.m=[]};h.i=function(){m(this)};function m(a){if(a.v)for(var b=o(a,new google.maps.LatLngBounds(a.a.getBounds().getSouthWest(),a.a.getBounds().getNorthEast())),c=0,e;e=a.b[c];c++){var g=false;if(!e.q&&b.contains(e.getPosition())){for(var k=0,j;j=a.m[k];k++)if(!g&&j.getCenter()&&j.s.contains(e.getPosition())){g=true;j.p(e);break}if(!g){j=new p(a);j.p(e);a.m.push(j)}}}}
function p(a){this.h=a;this.a=a.getMap();this.f=a.t();this.d=null;this.b=[];this.s=null;this.k=new q(this,a.u(),a.t())}h=p.prototype;
h.p=function(a){var b;a:if(this.b.indexOf)b=this.b.indexOf(a)!=-1;else{b=0;for(var c;c=this.b[b];b++)if(c==a){b=true;break a}b=false}if(b)return false;if(!this.d){this.d=a.getPosition();r(this)}if(this.b.length==0){a.setMap(this.a);a.setVisible(true)}else if(this.b.length==1){this.b[0].setMap(null);this.b[0].setVisible(false)}a.q=true;this.b.push(a);if(this.a.getZoom()>this.h.C())for(a=0;b=this.b[a];a++){b.setMap(this.a);b.setVisible(true)}else if(this.b.length<2)s(this.k);else{a=this.h.u().length;
b=this.h.B()(this.b,a);this.k.setCenter(this.d);a=this.k;a.w=b;a.da=b.text;a.Z=b.index;if(a.c)a.c.innerHTML=b.text;b=Math.max(0,a.w.index-1);b=Math.min(a.j.length-1,b);b=a.j[b];a.H=b.url;a.g=b.height;a.o=b.width;a.F=b.aa;a.anchor=b.$;a.G=b.ba;this.k.show()}return true};h.getBounds=function(){r(this);return this.s};h.remove=function(){this.k.remove();delete this.b};h.getCenter=f("d");function r(a){a.s=o(a.h,new google.maps.LatLngBounds(a.d,a.d))}h.getMap=f("a");
function q(a,b,c){a.h.extend(q,google.maps.OverlayView);this.j=b;this.ca=c||0;this.l=a;this.d=null;this.a=a.getMap();this.w=this.c=null;this.r=false;this.setMap(this.a)}h=q.prototype;
h.onAdd=function(){this.c=document.createElement("DIV");if(this.r){this.c.style.cssText=t(this,u(this,this.d));this.c.innerHTML=this.w.text}this.getPanes().overlayImage.appendChild(this.c);var a=this;google.maps.event.addDomListener(this.c,"click",function(){var b=a.l.h;google.maps.event.trigger(b,"clusterclick",[a.l]);if(b.S&&a.a.getZoom()>=b.R){b=a.l.b;for(var c=[],e=0;e<b.length;e++)c.push(b[e].content!==undefined&&b[e].content!=""?b[e].content:b[e].title);(new google.maps.InfoWindow({content:c.join("<br>")})).open(a.a,
b[0])}else if(b.Y){a.a.panTo(a.l.getCenter());a.a.fitBounds(a.l.getBounds())}})};function u(a,b){var c=a.getProjection().fromLatLngToDivPixel(b);c.x-=parseInt(a.o/2,10);c.y-=parseInt(a.g/2,10);return c}h.draw=function(){if(this.r){var a=u(this,this.d);this.c.style.top=a.y+"px";this.c.style.left=a.x+"px"}};function s(a){if(a.c)a.c.style.display="none";a.r=false}h.show=function(){if(this.c){this.c.style.cssText=t(this,u(this,this.d));this.c.style.display=""}this.r=true};h.remove=function(){this.setMap(null)};
h.onRemove=function(){if(this.c&&this.c.parentNode){s(this);this.c.parentNode.removeChild(this.c);this.c=null}};h.setCenter=d("d");
function t(a,b){var c=[];document.all?c.push('filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=scale,src="'+a.H+'");'):c.push("background:url("+a.H+");");if(typeof a.e==="object"){typeof a.e[0]==="number"&&a.e[0]>0&&a.e[0]<a.g?c.push("height:"+(a.g-a.e[0])+"px; padding-top:"+a.e[0]+"px;"):c.push("height:"+a.g+"px; line-height:"+a.g+"px;");typeof a.e[1]==="number"&&a.e[1]>0&&a.e[1]<a.o?c.push("width:"+(a.o-a.e[1])+"px; padding-left:"+a.e[1]+"px;"):c.push("width:"+a.o+"px; text-align:center;")}else c.push("height:"+
a.g+"px; line-height:"+a.g+"px; width:"+a.o+"px; text-align:center;");c.push("cursor:pointer; top:"+b.y+"px; left:"+b.x+"px; color:"+(a.F?a.F:"black")+"; position:absolute; font-size:"+(a.G?a.G:11)+"px; font-family:Arial,sans-serif; font-weight:bold");return c.join("")}window.MarkerClusterer=i;i.prototype.addMarker=i.prototype.p;i.prototype.addMarkers=i.prototype.z;i.prototype.clearMarkers=i.prototype.K;i.prototype.getCalculator=i.prototype.B;i.prototype.getGridSize=i.prototype.t;
i.prototype.getMap=i.prototype.getMap;i.prototype.getMarkers=i.prototype.L;i.prototype.getMaxZoom=i.prototype.C;i.prototype.getStyles=i.prototype.u;i.prototype.getTotalClusters=i.prototype.M;i.prototype.getTotalMarkers=i.prototype.N;i.prototype.redraw=i.prototype.i;i.prototype.removeMarker=i.prototype.U;i.prototype.resetViewport=i.prototype.n;i.prototype.setCalculator=i.prototype.V;i.prototype.setGridSize=i.prototype.W;i.prototype.onAdd=i.prototype.onAdd;i.prototype.draw=i.prototype.draw;
i.prototype.idle=i.prototype.O;q.prototype.onAdd=q.prototype.onAdd;q.prototype.draw=q.prototype.draw;q.prototype.onRemove=q.prototype.onRemove;})();
