        public async void ForgotPasswordEmail(string email, string token)
        {
            var from = new EmailAddress("fakeEmail@dispostable.com", "Example User");
            var subject = "Change password";
            var to = new EmailAddress(email);
            var plainTextContent = "Forgot password link";
            var htmlContent = ForgotEmailTemplate(token, email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await SendEmail(msg);
        }
        public string ForgotEmailTemplate(string token, string email)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "EmailTemplates", "ForgotPassword.html");
            string domain = _appKeys.Domain;
            string forgotPassword = File.ReadAllText(path).Replace("{{domain}}", domain).Replace("{{token}}", token).Replace("{{email}}", email);
            return forgotPassword;
        }       
