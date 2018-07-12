//All functions have a nameconfirm however only the password uses it and it is optional
function checkPasswordMatch(name, nameConfirm) {
    //Get elements
    var inpPassword = $(name);
    var inpPasswordConfirm = $(nameConfirm);
    //Password validation is done serverside since it already has a function for that
    if (inpPassword.val() == "") {
        //If there is nothing them show no extra styling
        inpPassword.add(inpPasswordConfirm).removeClass("is-valid is-invalid");
        return false;
    }
    else if (inpPassword.val() == inpPasswordConfirm.val()) //Check if they match and are not empty
    {
        //Show success
        inpPassword.add(inpPasswordConfirm).addClass("is-valid").removeClass("is-invalid");
        return true;
    }
    else {
        //Show error if they are not empty
        inpPassword.add(inpPasswordConfirm).removeClass("is-valid").addClass("is-invalid");
        return false;
    }
}
function checkEmailRule(name, nameConfirm) {
    //Get element
    var inpEmail = $(name);
    //Create regex for email
    var emailRegex = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/i;
    //Check it against the regex
    if (inpEmail.val() == "") {
        inpEmail.removeClass("is-valid is-invalid");
        return false;
    } else if (emailRegex.test(inpEmail.val())) {
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
function checkUsernameRule(name, nameConfirm) {
    //Get element
    var inpUsername = $(name);
    //Create regex for alphanumeric characters only
    var usernameRegex = /^[a-z0-9]+$/i;
    if (inpUsername.val() == "") {
        //If the field is empty show them no extra styling
        inpUsername.removeClass("is-valid is-invalid");
        return false;
    } else if (usernameRegex.test(inpUsername.val())) {
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

//Create the wrapped functions
function wrappedUsername(sender, args) {
    wrapperMatch(sender, args, checkUsernameRule(inpUsername));
}

function wrappedPassword(sender, args) {
    wrapperMatch(sender, args, checkPasswordMatch(inpPassword, inpRePassword));
}

function wrappedEmail(sender, args) {
    wrapperMatch(sender, args, checkEmailRule(inpEmail));
}