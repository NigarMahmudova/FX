

var checkCount = function () {
    let card_list = JSON.parse(localStorage.getItem('cards'));
    if (card_list.length > 0) {
        document.querySelector('.empty-basket').classList.add('d-none');
        document.querySelector('#content').classList.remove('d-none');
    
    } else {
        document.querySelector('#content').classList.add('d-none');
        document.querySelector('.empty-basket').classList.remove('d-none');
    }
}
checkCount();


function getCards() {
    let card_list = JSON.parse(localStorage.getItem('cards'));
    let x = "";
    let total = 0;
    let count = 0;
    card_list.forEach(cr => {
        console.log(cr.Id);
        x += `
        <table id=${cr.Id} class="cart" cellspacing="0">
                                    <thead>
                                        <tr>
                                            <th class="product-remove"><span class="screen-reader-text">Remove
                                                    item</span></th>
                                            <th class="product-thumbnail"><span class="screen-reader-text">Thumbnail
                                                    image</span></th>
                                            <th class="product-name">Product</th>
                                            <th class="product-price">Price</th>
                                            <th class="product-quantity">Quantity</th>
                                            <th class="product-subtotal">Subtotal</th>
                                        </tr>
                                    </thead>
                                    <tbody>
    
                                        <tr class="fx-cart-form__cart-item cart_item">
                                            
                                            <td class="product-remove" style="cursor: pointer">
                                                <span class=" d-none">${cr.Id}</span>
                                                <i class="fa-regular fa-x" onclick="Delete(this)"></i>
                                            </td>
    


                                            <td class="product-thumbnail">
                                                <a href="#">
                                                    <img
                                                        width="600" height="734" src=${cr.Image}
                                                        class="attachment-woocommerce_thumbnail size-woocommerce_thumbnail" alt="a">
                                                </a>
                                            </td>
    
                                            <td class="product-name" data-title="Product">
                                                <a href="#">${cr.Name}</a>
                                            </td>
    
                                            <td class="product-price" data-title="Price">
                                                <span class="woocommerce-Price-amount amount"><bdi><span
                                                            class="woocommerce-Price-currencySymbol">$</span>220.00</bdi></span>
                                            </td>
    
                                            <td class="product-quantity" data-title="Quantity">
                                                <span class=" d-none">${cr.Id}</span>
                                                <div class="fx-quantity-buttons quantity">
                                                    <span class="fx-quantity-minus" onclick="Minus(this)"></span>
                                                    <input type="text" id="quantity" class="fx-quantity-input"
                                                        data-min="0" data-max="" value="${cr.Count}" inputmode="numeric">
                                                    <span class="fx-quantity-plus" onclick="Plus(this)"></span>
                                                </div>
                                            </td>
    
                                            <td class="product-subtotal" data-title="Subtotal">
                                                <span class="woocommerce-Price-amount amount"><bdi><span
                                                            class="woocommerce-Price-currencySymbol">$</span>${cr.Count * cr.Price}</bdi></span>
                                            </td>
                                        </tr>
    
    
                                        <tr>
                                            <td colspan="6" class="actions">
    
                                                <div class="coupon">
                                                    <label for="coupon_code" class="d-none">Coupon:</label> 
                                                    <input type="text"
                                                        name="coupon_code" class="input-text" id="coupon_code" value=""
                                                        placeholder="Coupon code"> <button type="submit"
                                                        class="button wp-element-button" name="apply_coupon"
                                                        value="Apply coupon">Apply coupon</button>
                                                </div>
    
                                                <button type="submit" class="button wp-element-button" name="update_cart"
                                                    value="Update cart" disabled="" aria-disabled="true">Update
                                                    cart</button>
    
    
                                                <input type="hidden" id="woocommerce-cart-nonce"
                                                    name="woocommerce-cart-nonce" value="23f1453aa6"><input type="hidden"
                                                    name="_wp_http_referer" value="/cart/">
                                            </td>
                                        </tr>
    
                                    </tbody>
                                </table>
        `
        count += cr.Count
        total += (cr.Count * cr.Price)

    });
    document.querySelector('.rowlist').innerHTML = x;
    document.querySelector('#total-count').innerHTML = count;
    document.querySelector('#total').innerHTML = total;
}
getCards();

document.querySelector('#btn-clear').addEventListener('click', function () {
    localStorage.removeItem('cards');
    location.reload();
})

var Delete = function(x){
    let card_list = JSON.parse(localStorage.getItem('cards'));
    let wanted = x.previousElementSibling.innerHTML;
    let exist_prod = card_list.find(pr => pr.Id === wanted);
    let filtered_list = card_list.filter(pr => pr.Id != exist_prod.Id);
    localStorage.setItem('cards',JSON.stringify(filtered_list));
    getCards();
    ShowCount();
    checkCount();
}



var Minus = function(x){
    let card_list = JSON.parse(localStorage.getItem('cards'));
    // console.log(x.parentElement.previousElementSibling.innerHTML)
    let min = x.parentElement.previousElementSibling.innerHTML;
        
    if(card_list.length>1){
        let exist_prod = card_list.find(pr => pr.Id === min);
        if(Number(exist_prod.Count)>1){
            exist_prod.Count -=1;
            localStorage.setItem('cards',JSON.stringify(card_list));
        }else{
        let filtered_list = card_list.filter(pr => pr.Id != exist_prod.Id);
        localStorage.setItem('cards',JSON.stringify(filtered_list));
            
    }
    }else{
        let exist_prod = card_list.find(pr => pr.Id === min);
    if(Number(exist_prod.Count)>1){
        exist_prod.Count -=1;
        localStorage.setItem('cards',JSON.stringify(card_list));
    }else{
        localStorage.removeItem('cards')
        location.reload();
    }
    }
    getCards()
    ShowCount();
    checkCount();
}



var Plus = function(x){
    let card_list = JSON.parse(localStorage.getItem('cards'));
    console.log(x.parentElement.previousElementSibling.innerHTML)
    let id = x.parentElement.previousElementSibling.innerHTML
    let exist_prod = card_list.find(pr => pr.Id === id);
    exist_prod.Count += 1
    localStorage.setItem('cards',JSON.stringify(card_list));
    getCards()
}