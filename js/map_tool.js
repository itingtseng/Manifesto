function latlng_to_TW97(lon, lat) {

    //參考網址
    //http://wangshifuola.blogspot.tw/2010/08/twd97wgs84-wgs84twd97.html

    var a = 6378137.0;
    var b = 6356752.314245;
    var lon0 = 121 * Math.PI / 180;
    var k0 = 0.9999;
    var dx = 250000;

    var TWD97 = "";
    var lon = (lon / 180) * Math.PI;
    var lat = (lat / 180) * Math.PI;

    //---------------------------------------------------------
    var e = Math.pow((1 - Math.pow(b,2) / Math.pow(a,2)), 0.5);
    var e2 = Math.pow(e,2)/(1-Math.pow(e,2)); 
    var n = ( a - b ) / ( a + b );
    var nu = a / Math.pow((1-(Math.pow(e,2)) * (Math.pow(Math.sin(lat), 2) ) ) , 0.5);
    var p = lon - lon0;
    var A = a * (1 - n + (5/4) * (Math.pow(n,2) - Math.pow(n,3)) + (81/64) * (Math.pow(n, 4)  - Math.pow(n, 5)));
    var B = (3 * a * n/2.0) * (1 - n + (7/8.0)*(Math.pow(n,2) - Math.pow(n,3)) + (55/64.0)*(Math.pow(n,4) - Math.pow(n,5)));
    var C = (15 * a * (Math.pow(n,2))/16.0)*(1 - n + (3/4.0)*(Math.pow(n,2) - Math.pow(n,3)));
    var D = (35 * a * (Math.pow(n,3))/48.0)*(1 - n + (11/16.0)*(Math.pow(n,2) - Math.pow(n,3)));
    var E = (315 * a * (Math.pow(n,4))/51.0)*(1 - n);

    var S = A * lat - B * Math.sin(2 * lat) +C * Math.sin(4 * lat) - D * Math.sin(6 * lat) + E * Math.sin(8 * lat);
 
    //計算Y值
    var K1 = S*k0;
    var K2 = k0*nu*Math.sin(2*lat)/4.0;
    var K3 = (k0*nu*Math.sin(lat)*(Math.pow(Math.cos(lat),3))/24.0) * (5 - Math.pow(Math.tan(lat),2) + 9*e2*Math.pow((Math.cos(lat)),2) + 4*(Math.pow(e2,2))*(Math.pow(Math.cos(lat),4)));
    var y = K1 + K2*(Math.pow(p,2)) + K3*(Math.pow(p,4));
 
    //計算X值
    var K4 = k0 * nu * Math.cos(lat);
    var K5 = (k0 * nu * (Math.pow(Math.cos(lat), 3)) / 6.0) * (1 - Math.pow(Math.tan(lat), 2) + e2 * (Math.pow(Math.cos(lat), 2)));
    var x = K4 * p + K5 * (Math.pow(p, 3)) + dx;

    var result = {
        x: x,
        y: y
    }
    return result;

}