# OAuth Setup Instructions

This document explains how to set up Google OAuth authentication with Supabase.

## Current Implementation

The OAuth flow is set up as follows:
1. User clicks "Sign in with Google"
2. Frontend calls backend to get Google OAuth URL
3. User is redirected to Google for authentication
4. Google redirects back to `/auth/callback` with tokens in the URL hash
5. Frontend extracts tokens from URL and stores them
6. User is authenticated

## Supabase Configuration

### Required Steps

1. **Go to your Supabase Dashboard** at https://app.supabase.com

2. **Navigate to Authentication → Providers**

3. **Enable Google Provider:**
   - Click on "Google" in the providers list
   - Enable the provider
   - Add your Google OAuth credentials:
     - **Client ID**: From Google Cloud Console
     - **Client Secret**: From Google Cloud Console
   - **Redirect URL**: Make sure this matches exactly:
     ```
     http://localhost:5001/auth/callback
     ```
   - Save the settings

4. **Configure the Redirect URI in Supabase:**
   - Go to **Authentication → URL Configuration**
   - Add `http://localhost:5001/auth/callback` to the **Redirect URLs** list
   - Save the configuration

## Google Cloud Console Setup

1. **Go to Google Cloud Console** at https://console.cloud.google.com

2. **Select your project** (or create a new one)

3. **Enable Google+ API:**
   - Navigate to "APIs & Services" → "Library"
   - Search for "Google+ API" or "People API"
   - Enable the API

4. **Create OAuth 2.0 Credentials:**
   - Navigate to "APIs & Services" → "Credentials"
   - Click "Create Credentials" → "OAuth client ID"
   - Select "Web application"
   - **Authorized JavaScript origins**: Add:
     ```
     http://localhost:5001
     ```
   - **Authorized redirect URIs**: Add:
     ```
     https://swjqjykexyljmualexsw.supabase.co/auth/v1/callback
     ```
   - Click "Create"
   - Copy the **Client ID** and **Client Secret**

5. **Add to Supabase:**
   - In Supabase Dashboard → Authentication → Providers → Google
   - Paste the Client ID and Client Secret
   - Save

## Testing

1. **Start your backend:**
   ```bash
   cd BlogApp.Backend
   dotnet run
   ```

2. **Start your frontend:**
   ```bash
   cd BlogApp.Frontend
   dotnet run
   ```

3. **Test the OAuth flow:**
   - Navigate to http://localhost:5001/login
   - Click "Continue with Google"
   - You should be redirected to Google for authentication
   - After authenticating, you should be redirected back to your app
   - You should be logged in

## Troubleshooting

### Issue: Redirect URI mismatch
**Error**: "redirect_uri_mismatch"
**Solution**: 
- Make sure the redirect URL in Supabase matches exactly: `http://localhost:5001/auth/callback`
- Make sure the redirect URL in Google Cloud Console is: `https://swjqjykexyljmualexsw.supabase.co/auth/v1/callback`

### Issue: No token returned
**Error**: Authentication callback doesn't receive tokens
**Solution**:
- Check browser console for any JavaScript errors
- Verify the redirect URL in Supabase settings
- Check the Supabase logs in the dashboard

### Issue: Token expired
**Error**: Token is expired or invalid
**Solution**:
- The tokens are stored in localStorage
- Clear localStorage and try again
- Make sure the frontend is properly extracting tokens from the URL

## Configuration Values

Current configuration in `appsettings.json`:
```json
{
  "Supabase": {
    "Url": "https://swjqjykexyljmualexsw.supabase.co",
    "AnonKey": "eyJhbGci..."
  }
}
```

Make sure these values are correctly set in your Supabase project settings.

## Frontend Redirect URL

The frontend redirect URL is defined in:
- `BlogApp.Backend/Services/SupabaseService.cs` in the `GetGoogleSignInUrl()` method
- Current value: `http://localhost:5001/auth/callback`

If you need to change the port or domain, update this value in the code.

