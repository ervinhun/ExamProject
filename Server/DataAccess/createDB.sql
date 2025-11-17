-- DROP & CREATE SCHEMA
DROP SCHEMA IF EXISTS gameapp CASCADE;
CREATE SCHEMA IF NOT EXISTS gameapp;

-- ==============================
-- ROLES
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.roles (
                               id UUID PRIMARY KEY NOT NULL,
                               name VARCHAR(50) NOT NULL,
                               description TEXT,
                               is_deleted BOOLEAN DEFAULT FALSE
);

-- ==============================
-- USERS
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.users (
                               id UUID PRIMARY KEY NOT NULL,
                               full_name TEXT NOT NULL,
                               email TEXT UNIQUE NOT NULL,
                               password TEXT NOT NULL,
                               phone_no VARCHAR(100),
                               role_id UUID REFERENCES gameapp.roles(id),
                               jwt_token VARCHAR(255),
                               jwt_refresh_token VARCHAR(255),
                               is_active BOOLEAN DEFAULT TRUE,
                               active_status_expiry_date DATE,
                               created_at TIMESTAMP WITH TIME ZONE NOT NULL,
                               updated_at TIMESTAMP WITH TIME ZONE,
                               is_deleted BOOLEAN DEFAULT FALSE
);

-- ==============================
-- USER BALANCE
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.user_balance (
                                      id UUID PRIMARY KEY NOT NULL,
                                      user_id UUID NOT NULL REFERENCES gameapp.users(id),
                                      balance INTEGER NOT NULL,
                                      updated_at TIMESTAMP WITH TIME ZONE NOT NULL,
                                      is_deleted BOOLEAN DEFAULT FALSE
);

-- ==============================
-- TRANSACTIONS
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.transaction (
                                     id UUID PRIMARY KEY NOT NULL,
                                     user_id UUID NOT NULL REFERENCES gameapp.users(id),
                                     transaction_number TEXT NOT NULL,
                                     amount INTEGER NOT NULL,
                                     status VARCHAR(50) NOT NULL,   -- active/approved/rejected
                                     reviewed_by UUID REFERENCES gameapp.users(id),
                                     created_at TIMESTAMP WITH TIME ZONE NOT NULL,
                                     updated_at TIMESTAMP WITH TIME ZONE
);

-- ==============================
-- GAME
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.game (
                              id UUID PRIMARY KEY NOT NULL,
                              start_date DATE NOT NULL,
                              end_date DATE NOT NULL,
                              is_closed BOOLEAN DEFAULT FALSE,
                              winning_number_1 INTEGER,
                              winning_number_2 INTEGER,
                              winning_number_3 INTEGER,
                              created_at TIMESTAMP WITH TIME ZONE NOT NULL,
                              closed_at TIMESTAMP WITH TIME ZONE
);

-- ==============================
-- BOARD (template)
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.board (
                               id UUID PRIMARY KEY NOT NULL,
                               user_id UUID NOT NULL REFERENCES gameapp.users(id),
                               numbers TEXT NOT NULL,                -- "1,5,17,9,13"
                               field_count INTEGER NOT NULL,         -- 5–8
                               created_at TIMESTAMP WITH TIME ZONE NOT NULL,
                               is_deleted BOOLEAN DEFAULT FALSE
);

-- ==============================
-- PLAYING BOARD (actual game tickets)
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.playing_board (
                                       id UUID PRIMARY KEY NOT NULL,
                                       user_id UUID NOT NULL REFERENCES gameapp.users(id),
                                       board_id UUID NOT NULL REFERENCES gameapp.board(id),
                                       game_id UUID NOT NULL REFERENCES gameapp.game(id),
                                       numbers TEXT NOT NULL,
                                       field_count INTEGER NOT NULL,
                                       price INTEGER NOT NULL,
                                       is_repeat BOOLEAN DEFAULT FALSE,
                                       repeat_count_remaining INTEGER DEFAULT 0,
                                       is_winning_board BOOLEAN DEFAULT FALSE,
                                       created_at TIMESTAMP WITH TIME ZONE NOT NULL
);

-- ==============================
-- USER HISTORY
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.user_history (
                                      id BIGSERIAL PRIMARY KEY,
                                      public_id UUID NOT NULL,
                                      user_id UUID NOT NULL REFERENCES gameapp.users(id),
                                      title TEXT NOT NULL,
                                      description TEXT,
                                      created_at TIMESTAMP WITH TIME ZONE NOT NULL
);

-- ==============================
-- PLAYING HISTORY
-- ==============================
CREATE TABLE IF NOT EXISTS gameapp.playing_history (
                                         id BIGSERIAL PRIMARY KEY,
                                         public_id UUID NOT NULL,
                                         user_id UUID NOT NULL REFERENCES gameapp.users(id),
                                         ticket_id UUID NOT NULL REFERENCES gameapp.playing_board(id),
                                         title TEXT NOT NULL,
                                         description TEXT,
                                         created_at TIMESTAMP WITH TIME ZONE NOT NULL
);
