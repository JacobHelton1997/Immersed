        [HttpPut("forgot")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<int>> ForgotPassword(EmailForgotRequest email)
        {
            int code = 200;
            int id = 0;
            int tokenTypeId = (int)TokenType.ResetPassword;
            BaseResponse response = null;
            string token = null;

            try
            {
                id = _userService.GetIdByEmail(email.Email);
                token = Guid.NewGuid().ToString();
                response = new ItemResponse<int> { Item = id };
                _userService.AddUserToken(token, id, tokenTypeId);
                _emailsService.ForgotPasswordEmail(email.Email, token);                
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpPut("changepassword")]
        [AllowAnonymous]
        public ActionResult<SuccessResponse> ChangePassword(ChangePasswordRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            int tokenTypeId = (int) TokenType.ResetPassword;
            int userIdFromToken = _userService.GetUserFromToken(tokenTypeId, model.Token);
            int userIdFromEmail = _userService.GetIdByEmail(model.Email);
            bool areIdsMatching = false;
            if(userIdFromToken == userIdFromEmail)
            {
                areIdsMatching = true;
            }

            if (areIdsMatching)
            {
                try
                {
                    string salt = BCrypt.BCryptHelper.GenerateSalt();
                    string hashedPassword = BCrypt.BCryptHelper.HashPassword(model.Password, salt);
                    response = new SuccessResponse();
                    _userService.UpdateUserPassword(hashedPassword, userIdFromToken);
                    _userService.DeleteUserToken(model.Token);

                }

                catch(Exception ex)
                {
                    code = 500;
                    response = new ErrorResponse(ex.Message);
                }
            }
            else
            {
                code = 403;
                response = new ErrorResponse("Unauthorized to change password");
            }

            return StatusCode(code, response);
        }
