


let tabs = document.querySelectorAll('.tabs li');

for(let tab of tabs){
    tab.onclick = function(e){
        e.preventDefault();
        let active = document.querySelector('.product-active');
        
        active.classList.remove('product-active');
        this.classList.add('product-active');

        let data_id = this.getAttribute('data-id');
        let contents = document.querySelectorAll('.fx-Tabs-panel');

        for(let content of contents){
            if(content.id === data_id){
                content.classList.remove('d-none');
            }
            else{
                content.classList.add('d-none');
            }
        }
    }
}

let borders = document.querySelectorAll('.tabs li a');

for (let border of borders) {
    border.onclick = function (e) {
        e.preventDefault();
        let pr_border = document.querySelector('.product-border');

        pr_border.classList.remove('product-border');
        this.classList.add('product-border');
    }
}



// Add to cart

// if (localStorage.getItem('cards') === null) {
//     localStorage.setItem('cards', JSON.stringify([]))
// }

// let buttons = document.querySelectorAll('.js_add_to_cart');

// for(let btn of buttons) {
//     btn.onclick = function(e) {
//         e.preventDefault();
//         let id = this.parentElement.parentElement.id;
//         let src = this.parentElement.parentElement.children[0].children[1].src;
//         let cardName = this.parentElement.children[0].children[0].innerHTML;
//         let cardPrice = this.parentElement.children[1].children[0].children[1].innerHTML;
        
//         let card_list = JSON.parse(localStorage.getItem('cards'));

//         let existCard = card_list.find(item => item.Id === id);

//         if (existCard === undefined) {
//             card_list.push({
//                 Id: id,
//                 Image: src,
//                 Name: cardName,
//                 Price: cardPrice,
//                 Count: 1
//             })
//         } else {
//             existCard.Count++;
//         }

//         localStorage.setItem('cards', JSON.stringify(card_list));
//         // document.querySelector('.alert-box').style.display = 'block';
//         // setTimeout(() => {
//         // document.querySelector('.alert-box').style.display = 'none';
//         // }, 1000);
//         ShowCount();
//     }
// }

//  function ShowCount(){
//     let card_list = JSON.parse(localStorage.getItem('cards'));
//     document.querySelector('#count').innerHTML = card_list.length;
// }
// ShowCount();