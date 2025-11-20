# Login Issues Troubleshooting Guide

## Common Reasons for "Invalid Password" Error

### 1. User Not Registered Yet
**Problem**: You're trying to login with an account that doesn't exist in Supabase.

**Solution**: 
- First, register a new account at `/register`
- Then try logging in again

### 2. Email Not Confirmed
**Problem**: Supabase requires email confirmation by default.

**Solution**: 
- Check your email inbox for a confirmation email from Supabase
- Click the confirmation link
- Then try logging in again

**Disable Email Confirmation (for testing only)**:
1. Go to Supabase Dashboard
2. Navigate to **Authentication → Settings**
3. Disable **"Confirm email"**
4. Save the settings

### 3. Wrong Supabase Configuration
**Problem**: The Supabase credentials in `appsettings.json` might be incorrect.

**Check**:
- Verify the Supabase URL matches your project
- Verify the Anon Key is correct
- Make sure you're using the **anon key** (public key), not the service role key

### 4. Password Actually Doesn't Match
**Problem**: You might be entering the wrong password.

**Solution**:
- Try resetting the password:
  1. Click "Forgot your password?" on the login page
  2. Enter your email
  3. Check email for reset link
  4. Set a new password
  5. Try logging in again

### 5. Supabase Database Issues
**Problem**: The Supabase database might not be properly configured.

**Solution**:
1. Go to Supabase Dashboard
2. Check if your project is active
3. Verify the authentication schema exists
4. Check the logs in Supabase for any errors

## How to Debug

### Check Backend Logs
1. Run your backend: `dotnet run --project BlogApp.Backend`
2. Look at the console output when you try to login
3. Check for error messages like:
   - "SignIn failed for {Email}"
   - "Invalid login credentials"
   - Connection errors

### Check Supabase Logs
1. Go to [Supabase Dashboard](https://app.supabase.com)
2. Select your project
3. Navigate to **Logs**
4. Look for authentication-related errors

### Test the API Directly
Use a tool like Postman or curl to test the login endpoint:

```bash
curl -X POST http://localhost:64241/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"your-email@example.com","password":"your-password"}'
```

## Quick Fixes to Try

### Fix 1: Re-register with a New Account
1. Go to `/register`
2. Create a new account with a different email
3. Try logging in with the new credentials

### Fix 2: Check Supabase RLS (Row Level Security)
1. Go to Supabase Dashboard
2. Navigate to **Authentication → Policies**
3. Make sure there are policies that allow users to read their own data

### Fix 3: Reset Everything
1. Go to Supabase Dashboard
2. Navigate to **Authentication → Users**
3. Find your user
4. Click "Send password reset email"
5. Follow the reset process
6. Try logging in with the new password

## Register First if You Haven't

If you're trying to login with an account that doesn't exist, you'll always get "invalid credentials". 

**Make sure to**:
1. Navigate to `/register` (not `/login`)
2. Fill in your email, username, and password
3. Submit the form
4. If email confirmation is required, confirm your email
5. Then go to `/login` and use those credentials

## Contact Support

If none of the above works:
1. Check the backend console logs
2. Check the browser console (F12 → Console tab)
3. Look at the Network tab to see what error is being returned
4. Share the error messages you see

