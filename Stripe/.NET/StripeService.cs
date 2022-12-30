    public class StripeService: IStripeService
    {
        private AppKeys _appKeys = null;
        private IDataProvider _data = null;
        public StripeService(IOptions<AppKeys> appKeys, IDataProvider data)
        {
            _appKeys = appKeys.Value;
            _data = data;
        }
        public string CreateSession()
        {
            StripeConfiguration.ApiKey = _appKeys.StripeApiKey;
            string domain = _appKeys.Domain;

            SessionCreateOptions options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = "price_1MFlo2AgaIYgkkrxgh7JM7yR",
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = domain + "/checkoutsuccess?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "?canceled=true",
                InvoiceCreation = new SessionInvoiceCreationOptions { Enabled = true},
            };

            SessionService service = new SessionService();
            Session session = service.Create(options);

            return session.Id;
        }

        public string CreateSubscriptionSession(string priceId, string customerId)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeApiKey;
            string domain = _appKeys.Domain;

            SessionCreateOptions priceOptions = new SessionCreateOptions
            {
                Customer = customerId,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = priceId,
                        Quantity = 1,
                    },
                },

                Currency = "usd",
                Mode = "subscription",
                SuccessUrl = domain + "/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "?canceled=true",
            };

            SessionService service = new SessionService();
            Session session = service.Create(priceOptions);

            return session.Id;
        }

        public List<StripeProduct> GetSubscriptionsProducts()
        {
            string procName = "[dbo].[StripeProducts_SelectAll]";
            List<StripeProduct> list = null;

            _data.ExecuteCmd(procName, inputParamMapper: null,
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    StripeProduct product = MapSingleProduct(reader, ref startingIndex);

                    if (list == null)
                    {
                        list = new List<StripeProduct>();
                    }
                    list.Add(product);
                });
            return list;

        }

        public Paged<Subscription> GetSubscriptions(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[Subscriptions_SelectAll]";
            List<Subscription> list = null;
            Paged<Subscription> pagedList = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
            },
                delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    Subscription subscription = MapSingleSubscription(reader, ref startingIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex);
                    }
                    if (list == null)
                    {
                        list = new List<Subscription>();
                    }
                    list.Add(subscription);
                });
            if (list != null)
            {
                pagedList = new Paged<Subscription>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public CurrentSubscription GetSubscriptionByCreatedById(int createdById)
        {
            string procName = "[dbo].[Subscriptions_SelectCurrent]";
            CurrentSubscription subscription = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@CreatedById", createdById);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                 subscription = MapSingleCurrentSubscription(reader, ref startingIndex);
            });

            return subscription;
        }

        public int CreateSubscription(SubscriptionAddRequest model, int userId, string productId)
        {
            int id = 0;
            string procName = "[dbo].[Subscriptions_Insert]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddSubscriptionParams(model, col, userId, productId);

                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    col.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    Int32.TryParse(oId.ToString(), out id);
                });
            return id;
        }

        public int AddSubscription(string sessionId, int userId)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeApiKey;
            SessionService service = new SessionService();
            Session session = service.Get(sessionId);
            SubscriptionService subService = new SubscriptionService();
            Stripe.Subscription sub = subService.Get(session.SubscriptionId);

            SubscriptionAddRequest request = new SubscriptionAddRequest();

            request.SubscriptionId = session.SubscriptionId;
            request.CustomerId = session.CustomerId;
            request.DateEnded = sub.CurrentPeriodEnd;
            string productId = sub.Items.Data[0].Price.ProductId;
            if(sub.Status == "active")
            {
                request.IsActive = true;
            }
            else if(sub.Status == "canceled")
            {
                request.IsActive = false;
            }
            request.CreatedBy = userId;
            request.InvoiceId = sub.LatestInvoiceId;

            int id = CreateSubscription(request, userId, productId);

            return id;
        }

        public int AddTrasaction(string sessionId, int userId)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeApiKey;
            string domain = _appKeys.Domain;
            SessionService service = new SessionService();
            Session session = service.Get(sessionId);
            TransactionAddRequest request = new TransactionAddRequest();

            request.PaymentTypeId = 1;
            request.ExternalUserId = session.CustomerId;
            int number = (int) session.AmountTotal;
            decimal roundedNumber = decimal.Round((decimal)number/100,2 );
            request.AmountCharged = roundedNumber;
            request.ExternalTransactionId = session.PaymentIntentId;
            request.InvoiceId = session.InvoiceId;

            int id = CreateTransaction(request, userId);

            return id;
        }
        public Stripe.Subscription UpdateStripeSubscription(string subscriptionId, string priceId)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeApiKey;
            SubscriptionService subService = new SubscriptionService();
            Stripe.Subscription subscription = subService.Get(subscriptionId);
            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                ProrationBehavior = "create_prorations",
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Id = subscription.Items.Data[0].Id,
                        Price = priceId,
                    },
                },
            };

            subscription = subService.Update(subscription.Id, options);

            string invoiceId = subscription.LatestInvoiceId;

            string productId = subscription.Items.Data[0].Price.ProductId;

            UpdateSubscription(invoiceId, subscriptionId, productId);

            return subscription;
        }

        public void UpdateSubscription(string invoiceId, string subscriptionId, string productId)
        {
            string procName = "[dbo].[Subscriptions_Update]";
            _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@InvoiceId", invoiceId);
                col.AddWithValue("@SubscriptionId", subscriptionId);
                col.AddWithValue("@ProductId", productId);
            }, null);
        }

        public object GetInvoice(string customerId)
        {
            StripeConfiguration.ApiKey = _appKeys.StripeApiKey;

            InvoiceListOptions options = new InvoiceListOptions();
            {
                options.Limit = 1;
            }

            InvoiceService service = new InvoiceService();
            StripeList<Invoice> invoices = service.List(options);

            return invoices;
        }

        public Paged<Transaction> GetTransactionsByUserId(int pageIndex, int pageSize, int userId)
        {
            string procName = "[dbo].[Transactions_SelectByCreatedBy]";
            List<Transaction> list = null;
            Paged<Transaction> pagedList = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
                paramCollection.AddWithValue("@UserId", userId);
            },
                delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    Transaction transaction = MapSingleTransaction(reader, ref startingIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex);
                    }
                    if (list == null)
                    {
                        list = new List<Transaction>();
                    }
                    list.Add(transaction);
                });
            if (list != null)
            {
                pagedList = new Paged<Transaction>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public Transaction GetTransactionById(int id)
        {
            string procName = "[dbo].[Transactions_SelectById]";
            Transaction transaction = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                transaction = MapSingleTransaction(reader, ref startingIndex);
            });

            return transaction;
        }

        public int CreateTransaction(TransactionAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[Transactions_Insert]";

            _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection col)
            {
                AddTransactionParams(model, col, userId);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            }, delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;
                Int32.TryParse(oId.ToString(), out id);
            });
            return id;
        }

        public int CreatePaymentAccount(PaymentAccountAddRequest model, int userId)
        {
            int id = 0;
            string procName = "[dbo].[PaymentAccounts_Insert]";

            _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection col)
            {
                AddPaymentAccountParams(model, col);
                col.AddWithValue("@CreatedBy", userId);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;
                col.Add(idOut);
            }, delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;
                int.TryParse(oId.ToString(), out id);
            });
            return id;
        }

        public void UpdatePaymentAccount(PaymentAccountUpdateRequest model, int userId)
        {
            string procName = "[dbo].[PaymentAccounts_Update]";

            _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection col)
            {
                AddPaymentAccountParams(model, col);
                col.AddWithValue("@ModifiedBy", userId);
                col.AddWithValue("@Id", model.Id);
            }, null);
        }

        public Paged<PaymentAccount> GetPaymentAccounts(int pageIndex, int pageSize)
        {
            string procName = "[dbo].[PaymentAccounts_SelectAll]";
            List<PaymentAccount> list = null;
            Paged<PaymentAccount> pagedList = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
            },
                delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    PaymentAccount paymentAccount = MapSinglePaymentAccount(reader, ref startingIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex);
                    }
                    if (list == null)
                    {
                        list = new List<PaymentAccount>();
                    }
                    list.Add(paymentAccount);
                });
            if (list != null)
            {
                pagedList = new Paged<PaymentAccount>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public Paged<PaymentAccount> GetPaymentAccountsByCreatedBy(int pageIndex, int pageSize, int createdBy)
        {
            string procName = "[dbo].[PaymentAccounts_SelectByCreatedBy]";
            List<PaymentAccount> list = null;
            Paged<PaymentAccount> pagedList = null;
            int totalCount = 0;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);
                paramCollection.AddWithValue("@CreatedBy", createdBy);
            },
                delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    PaymentAccount paymentAccount = MapSinglePaymentAccount(reader, ref startingIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex);
                    }
                    if (list == null)
                    {
                        list = new List<PaymentAccount>();
                    }
                    list.Add(paymentAccount);
                });
            if (list != null)
            {
                pagedList = new Paged<PaymentAccount>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        public PaymentAccount GetPaymentAccountById(int id)
        {
            string procName = "[dbo].[PaymentAccounts_SelectById]";
            PaymentAccount paymentAccount = null;

            _data.ExecuteCmd(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                paymentAccount = MapSinglePaymentAccount(reader, ref startingIndex);
            });
            return paymentAccount;
        }

        public void DeletePaymentAccount(int id)
        {
            string procName = "[dbo].[PaymentAccounts_Delete]";

            _data.ExecuteNonQuery(procName, delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            }, null);
        } 

        private static StripeProduct MapSingleProduct(IDataReader reader, ref int startingIndex)
        {
            StripeProduct product = new StripeProduct();

            product.Name = reader.GetSafeString(startingIndex++);
            product.ProductId = reader.GetSafeString(startingIndex++);
            product.PriceId = reader.GetSafeString(startingIndex++);
            product.Price = reader.GetSafeDouble(startingIndex++);
            product.Description = reader.GetSafeString(startingIndex++);

            return product;
        }

        private static Subscription MapSingleSubscription(IDataReader reader, ref int startingIndex)
        {
            Subscription subscription = new Subscription();

            subscription.Id = reader.GetSafeInt32(startingIndex++);
            subscription.SubscriptionId = reader.GetSafeString(startingIndex++);
            subscription.CustomerId = reader.GetSafeString(startingIndex++);
            subscription.DateEnded = reader.GetSafeDateTime(startingIndex++);
            subscription.IsActive = reader.GetSafeBool(startingIndex++);
            subscription.CreatedBy = reader.GetSafeInt32(startingIndex++);
            subscription.DateCreated = reader.GetSafeDateTime(startingIndex++);
            subscription.DateModified = reader.GetSafeDateTime(startingIndex++);
            subscription.InvoiceId = reader.GetSafeString(startingIndex++);
            
            return subscription;
        }

        private static CurrentSubscription MapSingleCurrentSubscription(IDataReader reader, ref int startingIndex)
        {
            CurrentSubscription subscription = new CurrentSubscription();

            subscription.Id = reader.GetSafeInt32(startingIndex++);
            subscription.SubscriptionId = reader.GetSafeString(startingIndex++);
            subscription.CustomerId = reader.GetSafeString(startingIndex++);
            subscription.DateEnded = reader.GetSafeDateTime(startingIndex++);
            subscription.IsActive = reader.GetSafeBool(startingIndex++);
            subscription.CreatedBy = reader.GetSafeInt32(startingIndex++);
            subscription.InvoiceId = reader.GetSafeString(startingIndex++);
            subscription.PriceId = reader.GetSafeString(startingIndex++);

            return subscription;
        }

        private static void AddSubscriptionParams(SubscriptionAddRequest model, SqlParameterCollection col, int userId, string productId)
        {
            col.AddWithValue("@SubscriptionId", model.SubscriptionId);
            col.AddWithValue("@CustomerId", model.CustomerId);
            col.AddWithValue("@DateEnded", model.DateEnded);
            col.AddWithValue("@isActive", model.IsActive);
            col.AddWithValue("@CreatedBy", userId);
            col.AddWithValue("@InvoiceId", model.InvoiceId);
            col.AddWithValue("@ProductId", productId);
        }

        private static Transaction MapSingleTransaction(IDataReader reader, ref int startingIndex)
        {
            Transaction transaction = new Transaction();

            transaction.Id = reader.GetSafeInt32(startingIndex++);
            transaction.PaymentTypeId = reader.GetSafeInt32(startingIndex++);
            transaction.ExternalTransactionId = reader.GetSafeString(startingIndex++);
            transaction.ExternalUserId = reader.GetSafeString(startingIndex++);
            transaction.AmountCharged = reader.GetSafeDecimal(startingIndex++);
            transaction.CreatedBy = reader.GetSafeInt32(startingIndex++);
            transaction.DateCreated = reader.GetSafeDateTime(startingIndex++);
            transaction.InvoiceId = reader.GetSafeString(startingIndex++);

            return transaction;
        }

        private static void AddTransactionParams(TransactionAddRequest model, SqlParameterCollection col, int userId)
        {
            col.AddWithValue("@PaymentTypeId", model.PaymentTypeId);
            col.AddWithValue("@ExternalTransactionId", model.ExternalTransactionId);
            col.AddWithValue("@ExternalUserId", model.ExternalUserId);
            col.AddWithValue("@AmountCharged", model.AmountCharged);
            col.AddWithValue("@CreatedBy", userId);
            col.AddWithValue("@InvoiceId", model.InvoiceId);
        }

        private static void AddPaymentAccountParams(PaymentAccountAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@VendorId", model.VendorId);
            col.AddWithValue("@AccountId", model.AccountId);
            col.AddWithValue("@PaymentTypeId", model.PaymentTypeId);
        }

        private static PaymentAccount MapSinglePaymentAccount(IDataReader reader, ref int startingIndex)
        {
            PaymentAccount paymentAccount = new PaymentAccount();

            paymentAccount.Id = reader.GetSafeInt32(startingIndex++);
            paymentAccount.VendorId = reader.GetSafeInt32(startingIndex++);
            paymentAccount.AccountId = reader.GetSafeString(startingIndex++);
            paymentAccount.PaymentTypeId = reader.GetSafeInt32(startingIndex++);
            paymentAccount.DateCreated = reader.GetSafeDateTime(startingIndex++);
            paymentAccount.DateModified = reader.GetSafeDateTime(startingIndex++);
            paymentAccount.CreatedBy = reader.GetSafeInt32(startingIndex++);
            paymentAccount.ModifiedBy = reader.GetSafeInt32(startingIndex++);

            return paymentAccount;
        }
    }
