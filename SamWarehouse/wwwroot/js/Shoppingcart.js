//Add a listener to the broswer window which will run the enclosed code once it finishes loading.
window.addEventListener('load', () => {
    //Find the cart modal button and add a listener which will open the method shown when
    //the button is pressed.
    document.getElementById("btnCartModal").addEventListener('click', () => showCartModal());

    document.getElementById("cartModalBody").innerHTML = htmlResult;
    let itemQtyForms = document.querySelectorAll(".item-qty-form");
    itenQtyForms.forEach((form) => {
        form.addEventListener("submit", (e) => {
            e.preventDefault();
        })
    });
});

//The method triggered by the modal button event listener.
async function showCartModal() {
    //Send a request to the endpoint provided inside the fetch request and store the response
    let result = await fetch("/ShoppingCart/Index");
    //Check if the request was successful or not. It will not be 
    //successful if the user is not logged in.
    if (result.status != 200) {
        //Redirect to the login controller and run the login endpoint method.
        location.href = "/Login/Login";
    }
    //Retrieve the HTML content from the body of the fetch request result.
    //This will be the details of the shopping cart partial view.
    let htmlResult = await result.text();
    //Find the cart modal body by using its ID and put the partial view html
    //between its html tags.
    document.getElementById("cartModalBody").innerHTML = htmlResult;
    //Searches through the HTML of the page and finds all the item-qty-form objects.
    let itemQtyForms = document.querySelectorAll(".item-qty-form");
    //Cycle through the colleciotn of forms. For each form and an event listener
    //that will trigger on the form submit action to tell the form not to do its default 
    //action. This means the form will not auto-submit or close the modal/window it is in.
    itemQtyForms.forEach((form) => {
        form.addEventListener('submit', (e) => {
            e.preventDefault();
        });
    });

    setupQuantityButtons();
    setupRemoveButtons();

    calculateCartTotal();

    //Attaching the functionality of the button if it is push there then run this code.
    let checkoutButton = document.querySelector("#btnCheckout"){
        if (checkoutButton != null){
        checkoutButton.addEventListener('click', (e) => finaliseCart(e))
    }
}
//Use JQUERY to find the modal by its ID (dont forget to use the # symbol before the ID name)
//Then run the modal-show command on the modal to make it visible.
$('#cartModal').modal('show');
}

//this button is in the shoppingCart partial.cshtml
async function finaliseCart(e) {
    //it has ok and cancle button. little button pop out when we press checkout 
    if (confirm("Complete checkoout?") == true) {
        // Get the cart id our of the value attribute of the button
        let id = parseInt(e.target.getAttribute("value"))
        // send a fetch request to the controller using the Id and store the responce
        let result = await fetch("/ShoppingCart/FinaliseCart/" + id);

        if (result.status != 200) {
            alert("Something went wrong! unable to finalise cart")

        }
        //run hide commond. when you finish the shopping and start new shopping again.  
    }
    else {
        $('#cartModal').modal('show');
    }
}

async function setupRemoveButtons() {
    //Searches through the HTML of the page and finds all the objects with the 
    //remove-button class on them.
    let removeButtons = document.querySelectorAll(".remove-button");
    //Cycle through each button and add a listener to each one that will trigger the method
    //called removeItem.
    removeButtons.forEach((button) => {
        //Get the value attribute from the button tags and store its value. This will be
        //the primary key we stored when setting the buttons up.
        let cartItemId = parseInt(button.getAttribute("value"));
        button.addEventListener("click", () => removeItem(cartItemId));
    });
}

async function removeItem(cartItemId) {
    //Craete a JSON object to hold the cart ID
    let cartItem = {
        Id: cartItemId
    };
    //Send a fetch request to the controller to request the item to be removed from the
    //database.
    let result = await fetch('/ShoppingCart/RemoveFromCart', {
        method: "DELETE",
        headers: { "content-type": "application/json" },
        body: JSON.stringify(cartItem)
    });
    //If the request fails, alert the user with an error message
    if (result.status != 200) {
        alert("Remove Failed")
        return;
    }
    //Refresh the cart modal.
    showCartModal();
}


async function setupQuantityButtons() {
    //Searches through the HTML of the page and finds all the objects with the 
    //minus - button class on them.
    let minusButtons = document.querySelectorAll(".minus-button");
    //Cycle through each button and add a listener to each one that will trigeer the method
    //called decreaseQuantity.
    minusButtons.forEach((button) => {
        button.addEventListener("click", (e) => decreaseQuantity(e));
    });

    //Searches through the HTML of the page and finds all the objects with the 
    //minus - button class on them.
    let plusButtons = document.querySelectorAll(".plus-button");
    //Cycle through each button and add a listener to each one that will trigeer the method
    //called increaseQuantity.
    plusButtons.forEach((button) => {
        button.addEventListener("click", (e) => increaseQuantity(e));
    });
}

async function increaseQuantity(e) {
    //Get the target of the event. Find the target's parent form then run the query
    //selector on the form to find the first input element and get its value.
    let cartItemId = parseInt(e.target.form.querySelector("input").value);
    //Do the same as above except find the item in the form using the .qty class and grab its
    //innerText which is the text between the tags of the element.
    let qty = parseInt(e.target.form.querySelector(".qty").innerText);
    //Increase the quantity and update the qty field in the form with the new value
    qty++;
    e.target.form.querySelector(".qty").innerText = qty;
    //Pass the new qty and cart Item ID to the method that will request the controller to update the
    //value in the database.

    let lineItem = e.target.form.querySelector(".lineTotal");
    let unitprice = parseFloat(lineItem.getAttribute("value"));
    let totalprice = qty * (unitprice * 100);
    lineItem.innerText = "Total :" + parseFloat(totalprice / 100);

    calculateCartTotal();

    //Pass the new qty and cart Item ID to the method that will request the controller to update the
    //value in the database.

    updateQuantity(qty, cartItemId);
}

async function decreaseQuantity(e) {
    //Get the target of the event. Find the target's parent form then run the query
    //selector on the form to find the first input element and get its value.
    let cartItemId = parseInt(e.target.form.querySelector("input").value);
    //Do the same as above except find the item in the form using the .qty class and grab its
    //innerText which is the text between the tags of the element.
    let qty = parseInt(e.target.form.querySelector(".qty").innerText);

    //If the quantity is already at 1, dont decrease it. We will remove it instead. 
    if (qty == 1) {
        removeItem(cartItemId);
        return;
    }

    //Increase the quantity and update the qty field in the form with the new value
    qty--;
    e.target.form.querySelector(".qty").innerText = qty;

    let lineItem = e.target.form.querySelector(".lineTotal");
    let unitprice = parseFloat(lineItem.getAttribute("value"));
    let totalprice = qty * (unitprice * 100);
    lineItem.innerText = "Total :" + parseFloat(totalprice / 100);

    calculateCartTotal();
    //Pass the new qty and cart Item ID to the method that will request the controller to update the
    //value in the database.
    updateQuantity(qty, cartItemId);
}

async function updateQuantity(qty, cartItemId) {
    //Create a Javascript object to hold the cartItemId and updated quantity. The fields of
    //this object need to match the shoppingCartItem model of our project.
    let updatedItem = {
        Quantity: qty,
        Id: cartItemId
    };
    //Send a fetch request to the Shopping Cart controller to run the Update Quanity
    //endpoint. The request will be a PUT (update) method and will pass the updatedItem
    //details in the request body.
    let response = await fetch("/ShoppingCart/UpdateQuantity", {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(updatedItem)
    });
    //If the request fails and sends an error response message. Log the error and reset the modal.
    if (response.status != 200) {
        alert("Update Quantity Failed")
        showCartModal();
    }


    async function calculateCartTotal() {
        //use the wuery selector to find all the line total paragraph element and store them in a list. //
        //we will use these later to add all the total together for the cart total.
        let lineItems = document.querySelectorAll(".lineTotal");
        let total = 0;
        // cycle through all the line items in a foreach loop.
        //--get a line price go to the parse float go to Html and get inner text.total colunm number-- 
        lineItems.forEach((item) => {
            //perform a split on the line item's text to retrive just the price section and store it 
            let linePrice = parseFloat(item.innerText.split(" : ")[1]) * 100;

            //perform a split on the line item's text to retrive just the price section and store it 
            // --increase total, fixed(2) is fixed up to the 2 decimal point--.

            total += linePrice.toFixed(2);
        })
        //put the 
        //three place need to put- increase , decrease, short cut model.
        document.querySelector("#modalTotal").innerText = "Cart Total: " + (total / 100).toFixed(2);
    }
}
