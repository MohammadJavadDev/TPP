-- Create Database
CREATE DATABASE registrationdb;

-- Connect to the database
\c registrationdb;

-- Create Users Table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    fullname VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    phone VARCHAR(20) NOT NULL,
    password VARCHAR(64) NOT NULL,
    createdat TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Function to get all users
CREATE OR REPLACE FUNCTION get_all_users()
RETURNS SETOF users AS $$
BEGIN
    RETURN QUERY SELECT * FROM users ORDER BY id;
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error fetching users: %', SQLERRM;
END;
$$ LANGUAGE plpgsql;

-- Function to get user by ID
CREATE OR REPLACE FUNCTION get_user_by_id(user_id INT)
RETURNS SETOF users AS $$
BEGIN
    RETURN QUERY SELECT * FROM users WHERE id = user_id;
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error fetching user: %', SQLERRM;
END;
$$ LANGUAGE plpgsql;

-- Function to create a new user
CREATE OR REPLACE FUNCTION create_user(
    p_fullname VARCHAR,
    p_email VARCHAR,
    p_phone VARCHAR,
    p_password VARCHAR
)
RETURNS INT AS $$
DECLARE
    new_id INT;
BEGIN
    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM users WHERE email = p_email) THEN
        RAISE EXCEPTION 'Email already exists';
    END IF;
    
    INSERT INTO users (fullname, email, phone, password)
    VALUES (p_fullname, p_email, p_phone, p_password)
    RETURNING id INTO new_id;
    
    RETURN new_id;
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error creating user: %', SQLERRM;
END;
$$ LANGUAGE plpgsql;

-- Function to update a user
CREATE OR REPLACE FUNCTION update_user(
    p_id INT,
    p_fullname VARCHAR,
    p_email VARCHAR,
    p_phone VARCHAR,
    p_password VARCHAR
)
RETURNS BOOLEAN AS $$
BEGIN
    -- Check if user exists
    IF NOT EXISTS (SELECT 1 FROM users WHERE id = p_id) THEN
        RETURN FALSE;
    END IF;
    
    -- Check if email exists for another user
    IF EXISTS (SELECT 1 FROM users WHERE email = p_email AND id != p_id) THEN
        RAISE EXCEPTION 'Email already exists for another user';
    END IF;
    
    UPDATE users 
    SET fullname = p_fullname,
        email = p_email,
        phone = p_phone,
        password = p_password
    WHERE id = p_id;
    
    RETURN TRUE;
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error updating user: %', SQLERRM;
        RETURN FALSE;
END;
$$ LANGUAGE plpgsql;

-- Function to delete a user
CREATE OR REPLACE FUNCTION delete_user(p_id INT)
RETURNS BOOLEAN AS $$
BEGIN
    -- Check if user exists
    IF NOT EXISTS (SELECT 1 FROM users WHERE id = p_id) THEN
        RETURN FALSE;
    END IF;
    
    DELETE FROM users WHERE id = p_id;
    RETURN TRUE;
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error deleting user: %', SQLERRM;
        RETURN FALSE;
END;
$$ LANGUAGE plpgsql;

-- Function to check if an email exists
CREATE OR REPLACE FUNCTION check_email_exists(
    p_email VARCHAR,
    p_exclude_id INT DEFAULT NULL
)
RETURNS BOOLEAN AS $$
BEGIN
    IF p_exclude_id IS NULL THEN
        RETURN EXISTS (SELECT 1 FROM users WHERE email = p_email);
    ELSE
        RETURN EXISTS (SELECT 1 FROM users WHERE email = p_email AND id != p_exclude_id);
    END IF;
EXCEPTION
    WHEN OTHERS THEN
        RAISE EXCEPTION 'Error checking email: %', SQLERRM;
END;
$$ LANGUAGE plpgsql;