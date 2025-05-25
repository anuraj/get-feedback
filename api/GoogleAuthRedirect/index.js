const { OAuth2Client } = require('google-auth-library');
const client = new OAuth2Client();

module.exports = async function (context, req) {
    const token = req.body && req.body.token;
    if (!token) {
        context.res = {
            status: 400,
            body: 'Missing token.'
        };
        return;
    }
    try {
        // Verify the token with Google
        const ticket = await client.verifyIdToken({
            idToken: token,
            audience: '284242807372-7dd4tdh6plj40nuf1ikud02l7c9eadg6.apps.googleusercontent.com' // Your Google Client ID
        });
        const payload = ticket.getPayload();
        // Optionally, you can check payload fields here
        context.res = {
            status: 302,
            headers: {
                Location: '/web/dashboard.html'
            }
        };
    } catch (err) {
        context.res = {
            status: 401,
            body: 'Invalid token.'
        };
    }
};
