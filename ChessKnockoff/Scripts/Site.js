//Define the ID's
var inpUsernameID = "#inpUsername";
var inpPasswordID = "#inpPassword";
var inpRePasswordID = "#inpRePassword";
var inpEmailID = "#inpEmail";
var passwordFeedback = "#passwordFeedback";

//Checks whether the passwords match
function checkPasswordMatch() {
    //Get elements
    var inpPassword = $(inpPasswordID);
    var inpPasswordConfirm = $(inpRePasswordID);
    //Password validation is done serverside since it already has a function for that
    if (inpPassword.val() === "") {
        //If there is nothing them show no extra styling
        inpRePassword.removeClass("is-valid is-invalid");
        return false;
    }
    else if (inpPassword.val() === inpPasswordConfirm.val()) //Check if they match and are not empty
    {
        //Show success
        inpPasswordConfirm.addClass("is-valid").removeClass("is-invalid");
        return true;
    }
    else {
        //Show error if they are not empty
        inpPasswordConfirm.removeClass("is-valid").addClass("is-invalid");
        return false;
    }
}

//Checks whether the password is valid against the rules
function checkPasswordRule() {
    //Get password element
    var inpPassword = $(inpPasswordID);
    var fedPassword = $(passwordFeedback);

    //Stores the list of password erros
    var errorList = "";

    if (inpPassword.val() === "") {
        //If there is nothing them show no extra styling
        inpPassword.add(inpRePassword).removeClass("is-valid is-invalid");
        return false;
    } //Check the password rules

    if (inpPassword.val().length <= 6) {
        errorList += "Password must be larger than 6 characters. ";
    }

    if (inpPassword.val().length >= 256) //Check the length of the password
    {
        errorList += "Password must be shorter than 256 characters. ";
    }

    if (inpPassword.val() == inpPassword.val().toLowerCase()) { //Check if there is an upper case character
        errorList += "Password must contain upper case character. ";
    }

    if (!/\d/.test(inpPassword.val())) { //Check if there are any numbers
        errorList += "Password must contain a number. ";
    }

    if (!/[ !@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(inpPassword.val())) // //Check for special characters 
    {
        errorList += "Password must have a punctuation mark. ";
    }

    //Set the innhtml of the feedback to the error list
    fedPassword.html(errorList);

    //If there was no error
    if (errorList == "") {
        //Show success
        inpPassword.addClass("is-valid").removeClass("is-invalid");
        return true;
    }
    else
    {
        //Show error if any of the rules are invalid
        inpPassword.removeClass("is-valid").addClass("is-invalid");
        return false;
    }
}

//Checks whether the email is valid
function checkEmailRule() {
    //Get element
    var inpEmail = $(inpEmailID);
    //Create regex for email
    var emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
    //Check if it is empty
    if (inpEmail.val() === "") {
        inpEmail.removeClass("is-valid is-invalid");
        return false;
        //Check it against the regex and its length
    } else if (emailRegex.test(inpEmail.val()) && inpEmail.val().length <= 256) {
        //Show success
        inpEmail.addClass("is-valid");
        inpEmail.removeClass("is-invalid");
        return true;
    } else {
        //Show error
        inpEmail.removeClass("is-valid");
        inpEmail.addClass("is-invalid");
        return false;
    }
}

//Check whether the username is valid
function checkUsernameRule() {
    //Get element
    var inpUsername = $(inpUsernameID);
    //Create regex for alphanumeric characters only
    var usernameRegex = /^[a-z0-9]+$/i;

    //If the field is empty show them no extra styling
    if (inpUsername.val() === "") {
        inpUsername.removeClass("is-valid is-invalid");
        return false;
    //Check it agains the regex and the required length
    } else if (usernameRegex.test(inpUsername.val()) && inpUsername.val().length <= 25) {
        //Show success
        inpUsername.addClass("is-valid");
        inpUsername.removeClass("is-invalid");
        return true;
    } else {
        //Show error
        inpUsername.removeClass("is-valid");
        inpUsername.addClass("is-invalid");
        return false;
    }
}

//Create a wrapper function for the password validation check
//Required since client validation passes an args object for the client side validation
//whilst functions are used for a keyup event in Jquery which does not pass args
function wrapperMatch(sender, args, rule, value, valueConfirm = null) {
    //Apply the validation and use args to communicate whether it is valid or not
    if (rule(value, valueConfirm)) {
        args.IsValid = true;
    } else {
        args.IsValid = false;
    }
}

//Create the wrapped functions that interface with the client side validators
function wrappedUsername(sender, args) {
    wrapperMatch(sender, args, checkUsernameRule, inpUsername);
}

function wrappedPassword(sender, args) {
    wrapperMatch(sender, args, checkPasswordMatch, inpPassword, inpRePassword);
}

function wrappedEmail(sender, args) {
    wrapperMatch(sender, args, checkEmailRule, inpEmail);
}

function wrappedPasswordRule(sender, args) {
    wrapperMatch(sender, args, checkPasswordMatch, inpPassword);
}

//Create function to add the validation check if it exists on the page
function addValidation(control, validationFunction) {
    if (control.length > 0) {
        //Assign the functions to their respective keyup events so that it will immediately
        //Add the error class to the input and notify the user
        control.keyup(function () {
            validationFunction();
        });
    }
}

//Once the DOM has loaded
$(document).ready(function () {
    //Call the add validation emthod to add the validation to the page
    addValidation($(inpPasswordID).add(inpRePasswordID), checkPasswordMatch);
    addValidation($(inpUsernameID), checkUsernameRule);
    addValidation($(inpEmailID), checkEmailRule);
    addValidation($(inpPasswordID), checkPasswordRule)
});