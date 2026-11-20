// Docker sets API_PROXY_TARGET to the compose service. Host-side ng serve
// leaves it unset and continues to call the API on localhost.
const target = process.env.API_PROXY_TARGET || "http://localhost:21000";

module.exports = {
  "/api": {
    target,
    secure: false,
  },
};
