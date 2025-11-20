-- Blog posts table
create table if not exists public.posts (
    id uuid primary key default gen_random_uuid(),
    title text not null,
    excerpt text not null,
    content text not null,
    featured_image_url text,
    author_id uuid not null,
    status integer not null default 0,
    visibility integer not null default 0,
    created_at timestamptz not null default now(),
    published_at timestamptz,
    updated_at timestamptz,
    views_count integer not null default 0,
    likes_count integer not null default 0,
    comments_count integer not null default 0,
    reading_time_minutes integer not null default 1,
    slug text unique,
    meta_description text,
    meta_keywords text,
    scheduled_at timestamptz,
    tags text[],
    categories text[],
    is_ai_generated boolean not null default false,
    ai_prompt text,
    ai_confidence double precision not null default 0
);

-- Comments table
create table if not exists public.comments (
    id uuid primary key default gen_random_uuid(),
    post_id uuid not null references public.posts(id) on delete cascade,
    author_id uuid not null,
    content text not null,
    parent_comment_id uuid references public.comments(id) on delete cascade,
    created_at timestamptz not null default now(),
    updated_at timestamptz,
    likes_count integer not null default 0,
    is_deleted boolean not null default false
);

-- Likes table
create table if not exists public.post_likes (
    id uuid primary key default gen_random_uuid(),
    post_id uuid not null references public.posts(id) on delete cascade,
    user_id uuid not null,
    created_at timestamptz not null default now(),
    unique (post_id, user_id)
);

-- Helpful indexes
create index if not exists idx_posts_slug on public.posts using btree (slug);
create index if not exists idx_posts_author on public.posts using btree (author_id);
create index if not exists idx_posts_created_at on public.posts using btree (created_at desc);
create index if not exists idx_comments_post on public.comments using btree (post_id);

-- Additional governance columns for moderation and review
alter table if exists public.posts
    add column if not exists review_notes text;

alter table if exists public.comments
    add column if not exists is_approved boolean not null default true,
    add column if not exists is_flagged boolean not null default false,
    add column if not exists flag_reason text;

-- Site settings table for administrative configuration
create table if not exists public.site_settings (
    id uuid primary key default gen_random_uuid(),
    site_title text not null default 'My Blog',
    site_description text not null default 'A modern blog application',
    site_url text not null default 'https://myblog.com',
    logo_url text,
    favicon_url text,
    theme_color text default '#1976d2',
    primary_color text default '#1976d2',
    secondary_color text default '#424242',
    allow_comments boolean not null default true,
    require_comment_approval boolean not null default false,
    allow_user_registration boolean not null default true,
    require_email_verification boolean not null default true,
    contact_email text,
    social_media_links text,
    google_analytics_id text,
    facebook_pixel_id text,
    posts_per_page integer not null default 10,
    enable_newsletter boolean not null default true,
    enable_search boolean not null default true,
    updated_at timestamptz not null default now()
);


