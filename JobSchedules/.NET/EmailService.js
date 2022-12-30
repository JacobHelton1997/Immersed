        public async void PhishingEmail(string token, PhishingAddRequest model)
        {
            var fromEmail = new EmailAddress() 
            { 
                Email = model.FromEmail,
                Name = model.FromName
            };
        

            var toEmail = new EmailAddress()
            {
                Email = model.ToEmail,
                Name = model.ToName
            };

            var htmlContent = PhishingTemplate(token, model.ToEmail);
            var msg = MailHelper.CreateSingleEmail(fromEmail, toEmail, model.Subject, model.Body, htmlContent);
            await SendEmail(msg);
           
        }

        public string PhishingTemplate(string token, string email)
        {
            int tokenTypeId = (int)TokenType.TrainingEvent;
            string tokenType = tokenTypeId.ToString();
            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "EmailTemplates", "PhishingEmail.html");
            string domain = _appKeys.Domain;
            string phishing = File.ReadAllText(path).Replace("{{domain}}", domain).Replace("{{token}}", token).Replace("{{email}}", email).Replace("{{tokenTypeId}}", tokenType);
            return phishing;
        }
