# Re-Register

This app demonstrates a novel concept with regard to user registration and data storage.

The registration process doesn't save any user information to the database, but instead generates a JWT token that contains a base64 encoding of a JSON for the registration model **(in this case, UserViewmodel)**. The token is then emailed to the email address provided in the registration model as a querystring parameter for the link to the confirmation page.

This way, we save the user's data temporarily for use on the account Confirmation page later.

Once the user clicks on the link, we decode the registration model back from the base64 string that was "tokenized" over to the same registration model that was used to generate the token value.

The token is evaluated and used with the UserManager to create the user account.

## Browsing the code

Pretty much everything is in the Pages folder - Confirm and Register pages. Other than that is a basic email service and DTO classes.

## I KNOW

That this isn't the purpose of JWT tokens, but I needed something that couldn't be easily fooled. In this case, if the token is artificially created by some other means (in particular without the app **secret**), the system *SHOULD* be unable to decode the token and therefore, will fail to register the user account.

So maybe its a weird way to use JWT tokens, but hey at least it cuts down on spam.
