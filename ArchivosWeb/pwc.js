$(document).ready(function () {
  $(".toggle-accordion").on("click", function () {
    var accordionId = $(this).attr("accordion-id"),
      numPanelOpen = $(accordionId + " .collapse.in").length;

    $(this).toggleClass("active");

    if (numPanelOpen == 0) {
      openAllPanels(accordionId);
    } else {
      closeAllPanels(accordionId);
    }
  });

  openAllPanels = function (aId) {
    console.log("setAllPanelOpen");
    $(aId + ' .panel-collapse:not(".in")').collapse("show");
  };
  closeAllPanels = function (aId) {
    console.log("setAllPanelclose");
    $(aId + " .panel-collapse.in").collapse("hide");
  };
  openAllPanels("#accordion");
});

$(".pwc-toggle").on("click", function () {
  var ch = $(this).parent().attr("id").trim().slice(-1);
  console.log(ch);
  $(".pwc-panel_item").removeClass("active");
  $("#toogle--" + ch).addClass("active");
  $(".pwc-info").removeClass("selected");
  var selected = $(".pwc-info--" + ch).addClass("selected");
  console.log(selected);
});

$(".pwc-toggle-new").on("click", function () {
  var ch = $(this).parent().attr("id").trim().slice(-1);
  console.log(ch);
  $(".pwc-news_item").removeClass("active");
  $("#toggle-new--" + ch).addClass("active");
  $(".pwc-new").removeClass("selected");
  var selected = $(".pwc-news--" + ch).addClass("selected");
  console.log(selected);
});
