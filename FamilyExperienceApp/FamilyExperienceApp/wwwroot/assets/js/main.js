$(document).on("click", ".modal-btn", function (e) {
    e.preventDefault();
    let url = $(this).attr("href");
    fetch(url).then(response => {
        if (response.ok) {
            return response.text()
        }
        else {
            alert("xeta bas verdi")
            return
        }
    })
        .then(data => {
            $("#quick-view-modal").html(data)
        })
    let modal = document.querySelector('#quick-view-modal');
    modal.classList.add('open');
    //$("#quick-view-modal").modal("open")
})

$(document).on("click", "#quick-view-close", function (e) {
    e.preventDefault();
    let modal = document.querySelector('#quick-view-modal');
    modal.classList.remove('open');
})


$(document).on("click", "#basketSubmitBtn", function (e) {
    e.preventDefault();
    var productId = document.getElementById("productId").value;
    var sizeId = document.getElementById("sizeId").value;
    fetch("/Product/AddToBasket?id=" + productId + "&sizeId=" + sizeId, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
    })
        .then(response => {
            let modal = document.querySelector('#quick-view-modal');
            modal.classList.remove('open');
            location.reload();
        //    return response.json();
        })
        //.then(data => {
        //    console.log(data.data);
        //})
        .catch(error => {
        });

})


//$(document).on("click", "#wishlistSubmitBtn", function (e) {
//    e.preventDefault();
//    var ws_productId = document.getElementById("ws_productId").value;
//    fetch("/Product/AddToWishlist?id=" + ws_productId, {
//        method: "POST",
//        headers: {
//            "Content-Type": "application/json"
//        },
//    })
//        .then(response => {
//            alert("Elave edildi");
//        })
//        .catch(error => {
//        });

//})