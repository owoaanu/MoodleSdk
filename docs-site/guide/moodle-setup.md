# Moodle Setup Guide

To use this SDK, you must first enable Web Services on your Moodle site and generate a security token.

## Step 1: Enable Web Services
1. Log in to your Moodle site as an Administrator.
2. Go to **Site administration** > **Server** > **Web services** > **Overview**.
3. Follow the sequence steps provided by Moodle:
   - **Enable web services**: Set to 'Yes' and save.
   - **Enable protocols**: Enable the **REST protocol**.

## Step 2: Create a Web Service User (Recommended)
While you can use an admin account, it is safer to create a specific user for API interactions.
1. Create a new user (or use an existing one).
2. Ensure the user has the capability `webservice/rest:use`.

## Step 3: Select/Create a Service
1. Go to **Site administration** > **Server** > **Web services** > **External services**.
2. You can use a built-in service or create a custom one.
3. Click **Functions** for your service and add the functions you intend to use (e.g., `core_user_create_users`, `core_course_get_courses`).
4. Ensure **Enabled** is checked.
5. (Optional) Check **Can download files** if needed.

## Step 4: Add the User to the Service
1. In the **External services** list, click **Authorized users** for your service.
2. Add your Web Service user to the list.

## Step 5: Generate a Token
1. Go to **Site administration** > **Server** > **Web services** > **Manage tokens**.
2. Click **Add**.
3. Select your user and your service.
4. Click **Save changes**.
5. Copy the generated **Token**. This is what you will use in the SDK.

> [!IMPORTANT]
> Keep your token secure. Anyone with this token can perform actions on your Moodle site as that user.
