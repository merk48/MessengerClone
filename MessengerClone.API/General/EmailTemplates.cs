using MessengerClone.Domain.Utils.Enums;

namespace MessengerClone.API.General
{
    public static class EmailTemplates
    {

        public static string GetVerificationCodeEmailBody(string verificationCode, int VerificationCodeKeyTime)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='ar'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background-color: #ffffff;
                            border-radius: 8px;
                            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            overflow: hidden;
                        }}
                        .header {{
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 20px;
                            text-align: center;
                        }}
                        .content {{
                            padding: 20px;
                            text-align: right; /* Align text to the right for Arabic */
                        }}
                        .code {{
                            display: inline-block;
                            font-size: 24px;
                            font-weight: bold;
                            color: #007bff;
                            padding: 10px 20px;
                            border: 2px solid #007bff;
                            border-radius: 5px;
                            margin: 20px 0;
                        }}
                        .footer {{
                            padding: 10px;
                            text-align: center;
                            font-size: 12px;
                            color: #999999;
                            background-color: #f9f9f9;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>تحقق من حسابك</h1>
                        </div>
                        <div class='content'>
                            <p>مرحبًا،</p>
                            <p>تم طلب رمز التحقق لحسابك. استخدم الرمز أدناه لإكمال عملية التحقق:</p>
                            <p>رمز التحقق ستنتهي صلاحيته بعد {VerificationCodeKeyTime} دقيقة من الآن.</p>
                            <div class='code'>{verificationCode}</div>
                            <p>إذا لم تكن قد طلبت هذا، يرجى تجاهل هذه الرسالة.</p>
                            <p>شكرًا لك!</p>
                        </div>
                        <div class='footer'>
                            <p>حقوق النشر © {DateTime.Now.Year} شركتنا. جميع الحقوق محفوظة.</p>
                        </div>
                    </div>
                </body>
                </html>
                ";
        }


        public static string GetEmailConfirmEmailBody(string callbackUrl)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>Email Confirmation</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            text-align: center;
                            padding: 20px;
                        }}
                        .container {{
                            background: white;
                            padding: 20px;
                            border-radius: 8px;
                            max-width: 500px;
                            margin: auto;
                            box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.1);
                        }}
                        .button {{
                            display: inline-block;
                            padding: 12px 20px;
                            font-size: 16px;
                            color: white !important;
                            background-color: #007bff;
                            text-decoration: none;
                            border-radius: 5px;
                            margin-top: 20px;
                        }}
                        .button:hover {{
                            background-color: #0056b3;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Confirm Your Email Address</h2>
                        <p>Thank you for signing up! Please confirm your email address by clicking the button below:</p>
                        <a href='{System.Net.WebUtility.HtmlEncode(callbackUrl)}' class='button'>Confirm Email</a>
                        <p>If you didn’t request this, you can safely ignore this email.</p>
                    </div>
                </body>
                </html>";
        }

        // [1]
        // * user first registration going
        // sendEmailToConfirmEmail API        will send code in email to enter in registration form and in response to client
        //                                    will check if code match will call [confirmEmail] Api
        // confirmEmail API                   will update account field of ConfirmedEmail to true  

        // [2]
        // * when user first registration going
        // sendEmailToConfirmPhoneNumber API  will send code in email to enter in registration form and in response to client
        //                                    will check if code match will call [confirmPhoneNumber] Api
        // confirmPhoneNumber API             will update account field of ConfirmedPhoneNumber to true         

        // [3]
        // * when user forget his password    in login form - user click on forget password option
        //                                    page: email entered
        //                                    client call [sendEmailToResetPassword] api
        // sendEmailToResetPassword API       will send code in email to enter in login form and in response to client

        //                                    page: waiting for code 
        //                                    will check if code match will redirect to another page
        //                                    page: have fields of new password and confirm password
        //                                    client will call [resetPassword] api
        //                                    then redirect to login page again (operation done)
        // resetPassword API                  will update account field of Password to the new one 


        // [4]
        // * when user logged already         user click on change email option 
        //                                    client called [sendEmailToChangeEmail] api
        //                                    page: Go see your email to change the email
        // sendEmailToChangeEmail API         will send url in email to another page that have the userId and token in query string 
        //                                    client will catch userId and token when user open the page
        //                                    page: has fields for new email and confirm email
        //                                    client will call [sendEmailToConfirmEmail] api with this new email
        //                                    will check if code match will call [changeEmail] api 
        //                                    then redirect to profile page again (operation done)
        // changeEmail API                    will update account field of Email to the new one and the normilized Email and also ConfirmedEmail to true for احتياط

        // [5]
        // * when user logged already         user click on change phonenumber option
        //                                    client called [sendEmailToChangePhoneNumber] api
        //                                    page: Go see your email to change the phonenumber
        // sendEmailToChangePhoneNumber API   will send url in email to another page that have the userId and token in query string 
        //                                    client will catch userId and token when user open the page
        //                                    page: has field for new phonenumber
        //                                    client will call [sendEmailToConfirmPhoneNumber] api with this new phonenumber
        //                                    will check if code match will call [changePhoneNumber] api
        //                                    then redirect to profile page again (operation done)
        // changePhoneNumber API              will update account field of PhoneNumber to the new one and also ConfirmedPhoneNumber to true for احتياط


        // [6]
        // * when user logged already         user click on change password option
        //                                    client called [sendEmailToChangePassword] api
        //                                    page: Go see your email to change the password
        // sendEmailToChangePassword API      will send code in email to enter in login form and in response to client
        //                                    client will catch userId and token when user open the page
        //                                    page: has fields for new password and confirm password
        //                                    will check if code match will call [changePassword] api
        //                                    then redirect to profile page again (operation done)
        // changePassword API                 will update account field of Password to the new one 






        // Confirm Email
        // [1] page: waiting for you to click in the url that sent in the email 
        //           send email to this email with url (https:localhost.4400/confirmemail.html?userId=1&token?hkhkhjjkkldskldfkjlsdjfdkjldskdkjdfj)
        //                                         this will redirect to page that will say Email confirmed!
        //                                         (so client will call the confirmEmail api)
        //                                         client will catch the userId and token and send a cinfirmEmailDto to confirmEmail Api

        // Change Email 
        // [1] send email to the old email have link
        //                                 this will redirect to page that have fields to enter new email then press change
        //                                                                                                           will call changeEmail api

        // Chnage PhoneNumber
        // [1] page: waiting for the code that sent in the sms (in client I got the code from the response)
        //     send sms to the old phone number have code
        //                               see the code and enter it 
        //                               then the client will check
        //                                                      if correct then it will go to page for entering the new phonenumber then press change
        //                                                                                                                                     will call changePhoneNumber api
        //                                                      if no -> give user option to resend -> sendEmailToConfirmPhoneNumber api

    }
}
