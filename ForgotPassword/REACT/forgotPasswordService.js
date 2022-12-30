const forgotPassword = (payload) => {
  const config = {
    method: "PUT",
    url: `${endpoint}/forgot`,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: {
      "Content-Type": "application/json",
    },
  };
  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
};

const changePassword = (payload) => {
  const config = {
    method: "PUT",
    url: `${endpoint}/changepassword`,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: {
      "Content-Type": "application/json",
    },
  };
  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
};
