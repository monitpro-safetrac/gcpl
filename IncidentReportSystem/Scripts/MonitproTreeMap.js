var labelType, useGradients, nativeTextSupport, animate;
var currentID;

(function () {
    var ua = navigator.userAgent,
        iStuff = ua.match(/iPhone/i) || ua.match(/iPad/i),
        typeOfCanvas = typeof HTMLCanvasElement,
        nativeCanvasSupport = (typeOfCanvas == 'object' || typeOfCanvas == 'function'),
        textSupport = nativeCanvasSupport
          && (typeof document.createElement('canvas').getContext('2d').fillText == 'function');
    //I'm setting this based on the fact that ExCanvas provides text support for IE
    //and that as of today iPhone/iPad current text support is lame
    labelType = (!nativeCanvasSupport || (textSupport && !iStuff)) ? 'Native' : 'HTML';
    nativeTextSupport = labelType == 'Native';
    useGradients = nativeCanvasSupport;
    animate = !(iStuff || !nativeCanvasSupport);
})();

var Log = {
    elem: false,
    write: function (text) {
        if (!this.elem)
            this.elem = document.getElementById('log');
        this.elem.innerHTML = text;
        this.elem.style.left = (500 - this.elem.offsetWidth / 2) + 'px';
    }
};

var json;
function init(j) {
  var Param=0;
  var activenode;
    //init data
    //var json = { "id": "root", "name": "Factory1", "data": { "$color": null, "id": null, "$area": 0, "idata": null }, "children": [{ "id": "division1", "name": "division1", "data": { "$color": "green", "id": "division1", "$area": 257, "idata": "" }, "children": null }] };
    json = JSON.parse(j);
    
    /*var json = {
        "children": [
          {
              "children": [
                {
                    "children": [
                    {
                        "children": [],
                        "data": {

                            "$color": "#8E7032",
                            "idata": "It is a sample data",
                            "$area": 200,
                            "id": "Boiler1-Parameter1",
                        },
                        "id": "Boiler1-Parameter1",
                        "name": "aParameter 1"
                    },
                  {
                      "children": [],
                      "data": {
                          "playcount": "276",
                          "$color": "#8E7032",

                          "$area": 200
                      },
                      "id": "Boiler1-Parameter2",
                      "name": "aParameter 2"
                  }


                    ],
                    "data": {
                        "playcount": "276",
                        "$color": "#8E7032",

                        "$area": 276
                    },
                    "id": "equipment-Boiler 1",
                    "name": "Bioler 1 in East"
                },
                {
                    "children": [],
                    "data": {
                        "playcount": "271",
                        "$color": "#906E32",

                        "$area": 271
                    },
                    "id": "equipment-Boiler 2",
                    "name": "Bioler 2 in East"
                }
              ],
              "data": {
                  "playcount": 547,
                  "$area": 547
              },
              "id": "division_east",
              "name": "East Division"
          },
          {
              "children": [
                {
                    "children": [],
                    "data": {
                        "playcount": "317",
                        "$color": "#7E8032",

                        "$area": 317
                    },
                    "id": "equipment-pump 1",
                    "name": "Pump 1 in North"
                },
                {
                    "children": [],
                    "data": {
                        "playcount": "290",
                        "$color": "#897532",

                        "$area": 290
                    },
                    "id": "equipment-pump 2",
                    "name": "Pump 2 in North"
                }
              ],
              "data": {
                  "playcount": 607,
                  "$area": 607
              },
              "id": "division_North",
              "name": "North Division"
          },
          {
              "children": [
                {
                    "children": [

                     {
                         "children": [],
                         "data": {
                             "playcount": "276",
                             "$color": "#8E7032",

                             "$area": 200
                         },
                         "id": "Generator1-Parameter1",
                         "name": "Parameter 1"
                     },
                    {
                        "children": [],
                        "data": {
                            "playcount": "276",
                            "$color": "#8E7032",

                            "$area": 200
                        },
                        "id": "Generator1-Parameter2",
                        "name": "Parameter 2"
                    },
                  {
                      "children": [],
                      "data": {
                          "playcount": "276",
                          "$color": "#8E7032",

                          "$area": 200
                      },
                      "id": "Generator1-Parameter3",
                      "name": "Parameter 3"
                  }

                    ],
                    "data": {
                        "playcount": "247",
                        "$color": "#9A6432",

                        "$area": 247
                    },
                    "id": "equipment-Generator 1",
                    "name": "Generator 1 in West"
                },
                {
                    "children": [],
                    "data": {
                        "playcount": "218",
                        "$color": "#A65832",

                        "$area": 218
                    },
                    "id": "equipment-Generator 2",
                    "name": "Generator 2 in West"
                },
                {
                    "children": [],
                    "data": {
                        "playcount": "197",
                        "$color": "#AE5032",

                        "$area": 197
                    },
                    "id": "equipment-Generator 3",
                    "name": "Generator 3 in West"
                },
                {
                    "children": [],
                    "data": {
                        "playcount": "194",
                        "$color": "#B04E32",

                        "$area": 194
                    },
                    "id": "equipment-Generator 4",
                    "name": "Generator 4 in West"
                }
              ],
              "data": {
                  "playcount": 856,
                  "$area": 856
              },
              "id": "division_West",
              "name": "West Division"
          },
          {
              "children": [
                {
                    "children": [],
                    "data": {
                        "playcount": "349",
                        "$color": "#718D32",

                        "$area": 349
                    },
                    "id": "equipment-water",
                    "name": "Water in South"
                }
              ],
              "data": {
                  "playcount": 349,
                  "$area": 349
              },
              "id": "division_South",
              "name": "South Division"
          }
        ],
        "data": {},
        "id": "root",
        "name": "MonitPro Factory TreeMap"
    };
    */
    //end
    //init TreeMap
    var tm = new $jit.TM.Squarified({
        //where to inject the visualization
        injectInto: 'infovis',
        //parent box title heights
        titleHeight: 30,
        //show only one tree level  
         levelsToShow: 2,  
        //enable animations
        animate: animate,
        //box offsets
        offset: 1,
        //Attach left and right click events
        Events: {
            enable: true,
            onClick: function (node,eventinfo,evt) {
                if($(evt.target).text()!="Trend")
                {
                    var innerhtml="";
                var data = node.data;
                //alert($(evt.target).text());
                if($(evt.target).text()!="Trend")

                tm.enter(node);
                
                
                if (data.idata && Param==0) {
                  activenode=node;
                  var res=data.idata;
                  innerhtml+="<table>";
                  innerhtml += "<tr><th colspan='5' width='18%'>Parameter Name</th><td colspan='7' width='82%'>" + res[0] + "</td></tr>";

                        innerhtml += "<tr><th colspan='5' >Description</th><td colspan='7' >" + res[1] + "</td></tr>";
                        innerhtml += "<tr><th colspan='5' >Reason For Monitoring</th><td colspan='7' >" + res[2] + "</td></tr>";


                        innerhtml += "<tr><th colspan='5' >LTR</th><td colspan='7' >" + res[3] + "</td></tr>";
                        innerhtml += "<tr><th colspan='5' >LTC</th><td colspan='7' >" + res[4] + "</td></tr>";
                        innerhtml += "<tr><th colspan='5' >LTA</th><td colspan='7' >" + res[5] + "</td></tr>";
                        innerhtml += "<tr><th colspan='5' >HTR</th><td colspan='7' >" + res[6] + "</td></tr>";
                        innerhtml += "<tr><th colspan='5' >HTC</th><td colspan='7' >" + res[7] + "</td></tr>";
                        innerhtml += "<tr><th colspan='5' >HTA</th><td colspan='7' >" + res[8] + "</td></tr>";

                        innerhtml += "<tr><th colspan='3'>LTV</th><th colspan='3'>HTV</th><th colspan='3'>MV</th><th colspan='3'>UOM</th></tr>";
                        innerhtml += "<tr><td colspan='3'>" + res[9] + "</td><td colspan='3'>" + res[10] + "</td><td colspan='3'>" + res[11] + "</td><td colspan='3'>" + res[12] + "</td></tr>";

                        innerhtml += "<tr><th colspan='3'>Who</th><th colspan='3'>Frequency</th><th colspan='3'>Priority</th><th colspan='3'>TagID</th></tr>";
                        innerhtml += "<tr><td colspan='3'>" + res[13] + "</td><td colspan='3'>"+res[14]+"</td><td colspan='3'>" + res[15] + "</td><td colspan='3'>" + res[16] + "</td></tr>";

                        innerhtml += "<tr><th colspan='12'>Notes</th></tr>";
                        innerhtml += "<tr><td colspan='12'>" + res[17] + "</td></tr>";
                        innerhtml+="</table>";
                    idata = data.idata;
                    var str=String(document.getElementById(node.id).innerHTML).split("<br>");
                    var temp=str[0];
                    document.getElementById(node.id).innerHTML ="";
                    var html = "<div class='description'>" + innerhtml + "</div>";
                    document.getElementById(node.id).innerHTML ='<span class="eqpname" id="eqpname">'+data.EqpName+'-</span>'+temp+'</div>';
                    document.getElementById(node.id).innerHTML += html;
                  Param=1;
                }

                }else
                {
                  var data = node.data;
                  var res=data.idata;
                  //alert(res[16]);
                  //window.open("/Chart/Trend?TagID="+res[16])
                }

            },
            onRightClick: function () {
                tm.out();
                
                if(activenode)
                {
                  
                  document.getElementById(activenode.id).innerHTML=activenode.name;
                  activenode=null;
                }
                Param=0;
                
                $(".description").hide();
                $(".eqpname").hide();

            }
        },
        duration: 1000,
        //Enable tips
        Tips: {
            enable: false,
            //add positioning offsets
            offsetX: 20,
            offsetY: 20,
            //implement the onShow method to
            //add content to the tooltip when a node
            //is hovered
            onShow: function (tip, node, isLeaf, domElement) {
                var html = "<div class=\"tip-title\">" + node.name
                  + "</div><div class=\"tip-text\">";
                var data = node.data;
                if (data.playcount) {
                    html += "play count: " + data.playcount;
                }
                if (data.image) {
                    html += "<img src=\"" + data.image + "\" class=\"album\" />";
                }
                //tip.innerHTML =  html; 
                tip.hide();
            }
        },
        //Add the name of the node in the correponding label
        //This method is called once, on label creation.
        onCreateLabel: function (domElement, node) {
 
                domElement.innerHTML = node.name;
                var style = domElement.style;
                style.display = '';
                style.border = '1px solid transparent';
            
            
            domElement.onmouseover = function () {
                style.border = '1px solid #9FD4FF';
            };
            domElement.onmouseout = function () {
                style.border = '1px solid transparent';
            };

        }
    });
    tm.loadJSON(json);
    tm.refresh();
    //end
    //add events to radio buttons
    var sq = $jit.id('r-sq'),
        st = $jit.id('r-st'),
        sd = $jit.id('r-sd');
    var util = $jit.util;
    util.addEvent(sq, 'change', function () {
        if (!sq.checked) return;
        util.extend(tm, new $jit.Layouts.TM.Squarified);
        tm.refresh();
    });
    util.addEvent(st, 'change', function () {
        if (!st.checked) return;
        util.extend(tm, new $jit.Layouts.TM.Strip);
        tm.layout.orientation = "v";
        tm.refresh();
    });
    util.addEvent(sd, 'change', function () {
        if (!sd.checked) return;
        util.extend(tm, new $jit.Layouts.TM.SliceAndDice);
        tm.layout.orientation = "v";
        tm.refresh();
    });
    //add event to the back button
    var back = $jit.id('back');
    $jit.util.addEvent(back, 'click', function () {
        tm.out();
    });
}
