//Define the ID's
var inpUsernameID = "#inpUsername";
var inpPasswordID = "#inpPassword";
var inpRePasswordID = "#inpRePassword";
var inpEmailID = "#inpEmail";

//Chekcs whether the passwords match
function checkPasswordMatch() {
    //Get elements
    var inpPassword = $(inpPasswordID);
    var inpPasswordConfirm = $(inpRePasswordID);
    //Password validation is done serverside since it already has a function for that
    if (inpPassword.val() === "") {
        //If there is nothing them show no extra styling
        inpPassword.add(inpRePassword).removeClass("is-valid is-invalid");
        return false;
    }
    else if (inpPassword.val() === inpPasswordConfirm.val()) //Check if they match and are not empty
    {
        //Show success
        inpPassword.add(inpRePassword).addClass("is-valid").removeClass("is-invalid");
        return true;
    }
    else {
        //Show error if they are not empty
        inpPassword.add(inpPasswordConfirm).removeClass("is-valid").addClass("is-invalid");
        return false;
    }
}
//Checks whether the email is valid
function checkEmailRule() {
    //Get element
    var inpEmail = $(inpEmailID);
    //Create regex for email
    var emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
    //Check it against the regex
    if (inpEmail.val() === "") {
        inpEmail.removeClass("is-valid is-invalid");
        return false;
    } else if (emailRegex.test(inpEmail.val()) && inpEmail.val().length <= 320) {
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
    if (inpUsername.val() === "") {
        //If the field is empty show them no extra styling
        inpUsername.removeClass("is-valid is-invalid");
        return false;
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

//Create a wrapper function to returns valid or invalid so that the original function can be called from the clientside validation which does not allow parameters
function wrapperMatch(sender, args, rule, name, nameConfirm = null) {
    if (rule(name, nameConfirm)) {
        args.IsValid = true;
    } else {
        args.IsValid = false;
    }
}

//Create the wrapped functions that interface with the client validators
function wrappedUsername(sender, args) {
    wrapperMatch(sender, args, checkUsernameRule, inpUsername);
}

function wrappedPassword(sender, args) {
    wrapperMatch(sender, args, checkPasswordMatch, inpPassword, inpRePassword);
}

function wrappedEmail(sender, args) {
    wrapperMatch(sender, args, checkEmailRule, inpEmail);
}

//Create function to add the validation check if it exists
function addValidation(control, validationFunction) {
    if (control.length > 0) {
        validationFunction();

        control.keyup(function () {
            validationFunction();
        });
    }
}

//Assign the events once the DOM has loaded
$(document).ready(function () {
    addValidation($(inpPasswordID).add(inpRePasswordID), checkPasswordMatch);
    addValidation($(inpUsernameID), checkUsernameRule);
    addValidation($(inpEmailID), checkEmailRule);
});