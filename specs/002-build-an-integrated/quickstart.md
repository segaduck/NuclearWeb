# Quickstart Guide: Integrated Multi-Module Platform

**Feature**: Integrated Multi-Module Web Application Platform
**Date**: 2025-10-03
**Purpose**: Validation guide for testing complete user workflows end-to-end

## Overview

This quickstart guide provides step-by-step instructions to validate all major user scenarios defined in the feature specification. It serves as both a testing checklist and user onboarding guide.

---

## Prerequisites

### Environment Setup

1. **Docker and Docker Compose** installed
2. **.NET 9 SDK** (for local development)
3. **Node.js 20+** and npm (for frontend development)
4. **Git** (for cloning repository)

### Initial Setup

```bash
# Clone repository
git clone <repository-url>
cd NuclearWeb

# Start all services via Docker Compose
docker-compose up -d

# Wait for services to be healthy (check logs)
docker-compose logs -f

# Services should be running on:
# - Backend API: http://localhost:5000
# - Frontend: http://localhost:3000
# - MySQL: localhost:3306
```

### Seed Test Data

```bash
# Run database migrations and seed data
docker-compose exec backend dotnet ef database update
docker-compose exec backend dotnet run --seed-data

# Test data created:
# - Admin user: admin / Admin@123
# - Normal user: john.doe / User@123
# - 5 meeting rooms
# - 3 sample content articles
# - 2 menu items
```

---

## User Journey 1: Administrator Complete Workflow

**Goal**: Validate all admin capabilities across both modules

### Step 1: Login as Administrator

1. Open browser: `http://localhost:3000`
2. Click "Login" button
3. Enter credentials:
   - Username: `admin`
   - Password: `Admin@123`
4. Click "Sign In"

**Expected Result**:
- ✅ Redirected to dashboard
- ✅ Sidebar shows all modules: Reservations, Knowledge Base, Admin Panel
- ✅ User menu shows "admin" display name

### Step 2: Theme and Sidebar Test

1. Click theme toggle button (sun/moon icon)
2. Verify UI switches to dark mode instantly
3. Refresh page
4. Click sidebar collapse button
5. Verify sidebar collapses to icon-only mode
6. Hover over icons to see tooltips
7. Click expand button

**Expected Result**:
- ✅ Theme switches without page reload (<100ms)
- ✅ Theme preference persists after refresh
- ✅ Sidebar collapse state smooth transition
- ✅ Tooltips appear on hover in collapsed mode
- ✅ Sidebar state persists

### Step 3: Create Meeting Room

1. Navigate to "Admin Panel" → "Meeting Rooms"
2. Click "Add New Room" button
3. Fill form:
   - Name: "Conference Room Z"
   - Capacity: 15
   - Location: "Building 2, Floor 1"
   - Amenities: Select "Projector", "Whiteboard", "Video Conference"
4. Click "Save"

**Expected Result**:
- ✅ Room created successfully
- ✅ Success notification displayed
- ✅ Room appears in room list
- ✅ Room immediately available for booking

### Step 4: Create Reservation (Calendar - Month View)

1. Navigate to "Reservations"
2. Verify calendar shows in month view by default
3. Click on tomorrow's date
4. Create reservation dialog appears
5. Fill form:
   - Room: "Conference Room Z"
   - Start Time: Tomorrow 10:00 AM
   - End Time: Tomorrow 12:00 PM
   - Purpose: "Strategic Planning Meeting"
   - Attendees: 10
6. Click "Book"

**Expected Result**:
- ✅ Reservation created
- ✅ Event appears on calendar immediately
- ✅ Conflict check validates no overlap
- ✅ Attendee count validated against room capacity

### Step 5: Test Calendar View Switching

1. Click "Week" view button
2. Verify calendar switches to week view (<200ms)
3. Verify reservation appears in week view
4. Click "Day" view button
5. Verify calendar switches to day view
6. Click "Month" view button

**Expected Result**:
- ✅ View switching is instant (no page reload)
- ✅ View switching completes in <200ms
- ✅ Reservation displays correctly in all views
- ✅ Selected date context preserved across views

### Step 6: Test Conflict Prevention

1. In calendar, click on tomorrow 10:30 AM (overlaps with existing)
2. Attempt to create reservation:
   - Room: "Conference Room Z"
   - Start: Tomorrow 10:30 AM
   - End: Tomorrow 11:30 AM
3. Click "Book"

**Expected Result**:
- ✅ Error message: "Room already booked for this time period"
- ✅ Shows conflicting reservation details
- ✅ Reservation NOT created
- ✅ Calendar remains unchanged

### Step 7: Edit Another User's Reservation (Admin Privilege)

1. Create a test reservation as admin
2. Note reservation ID
3. Logout and login as normal user (`john.doe / User@123`)
4. Create a reservation
5. Logout and login back as admin
6. Navigate to "Admin Panel" → "All Reservations"
7. Find john.doe's reservation
8. Click "Edit"
9. Change time and save

**Expected Result**:
- ✅ Admin can edit any user's reservation
- ✅ Modification history tracked
- ✅ Calendar updates with new time
- ✅ Conflict check still enforced

### Step 8: Create CMS Content Article

1. Navigate to "Knowledge Base" → "Content Management"
2. Click "New Article"
3. Fill form:
   - Title: "Safety Procedures Update"
   - Content: Use rich text editor
     - Add heading: "Emergency Response Protocol"
     - Add paragraph with bold and italic text
     - Insert image (upload test image)
     - Add YouTube video embed: `https://www.youtube.com/watch?v=dQw4w9WgXcQ`
     - Add bulleted list
4. Click "Save as Draft"

**Expected Result**:
- ✅ Article saved as Draft status
- ✅ All formatting preserved
- ✅ Image uploaded and displayed
- ✅ YouTube video embedded successfully
- ✅ Article appears in draft list

### Step 9: Submit and Approve Article (Approval Workflow)

1. From draft article list, click "Submit for Approval"
2. Verify status changes to "Pending Approval"
3. Navigate to "Admin Panel" → "Pending Articles"
4. Click on submitted article
5. Review content
6. Click "Approve" button
7. Set availability period:
   - Start: Today
   - End: (leave empty for indefinite)
8. Click "Publish"

**Expected Result**:
- ✅ Article status changes to "Published"
- ✅ Availability period saved
- ✅ Article appears on frontend Knowledge Base
- ✅ Published timestamp recorded

### Step 10: Create Menu Structure

1. Navigate to "Admin Panel" → "Menu Management"
2. Click "Add Main Menu"
3. Create main menu:
   - Name: "Safety"
   - Display Order: 1
4. Click "Save"
5. Click "Add Sub-Menu" under "Safety"
6. Create sub-menu:
   - Name: "Emergency Procedures"
   - Link Type: "Article"
   - Article: Select "Safety Procedures Update"
   - Display Order: 0
7. Click "Save"

**Expected Result**:
- ✅ Main menu created
- ✅ Sub-menu linked to article
- ✅ Menu appears in frontend navigation
- ✅ Dropdown shows sub-menu on hover
- ✅ Clicking sub-menu opens article

### Step 11: Upload and Manage Files

1. Navigate to "Admin Panel" → "File Management"
2. Click "Upload File"
3. Select test PDF file (< 100MB)
4. Fill metadata:
   - Description: "2025 Safety Manual"
   - Category: "Documents"
5. Click "Upload"
6. Verify file appears in file list
7. Click file name to download
8. Verify download starts and file is correct

**Expected Result**:
- ✅ File uploaded successfully
- ✅ File type validated (only allowed types)
- ✅ File size validated (<= 100MB)
- ✅ File listed in file browser
- ✅ Download works correctly
- ✅ Download counter increments

### Step 12: User Management

1. Navigate to "Admin Panel" → "Users"
2. Click "Add User"
3. Create user:
   - Username: `test.user`
   - Password: `Test@123`
   - Display Name: "Test User"
   - Email: `test.user@example.com`
   - Role: "User"
4. Click "Create"
5. Edit user and change role to "Admin"
6. Save changes
7. Deactivate user

**Expected Result**:
- ✅ User created successfully
- ✅ Username uniqueness validated
- ✅ Email uniqueness validated
- ✅ Role change persisted
- ✅ Deactivated user cannot login
- ✅ User marked as inactive (soft delete)

---

## User Journey 2: Normal User Workflow

**Goal**: Validate normal user capabilities and restrictions

### Step 1: Login as Normal User

1. Logout from admin account
2. Login with:
   - Username: `john.doe`
   - Password: `User@123`

**Expected Result**:
- ✅ Login successful
- ✅ Sidebar shows limited modules: Reservations, Knowledge Base (no Admin Panel)

### Step 2: Create Own Reservation

1. Navigate to "Reservations"
2. Create reservation:
   - Room: "Conference Room A"
   - Start: Day after tomorrow 2:00 PM
   - End: Day after tomorrow 3:00 PM
   - Purpose: "Team Meeting"
3. Click "Book"

**Expected Result**:
- ✅ Reservation created
- ✅ User ID associated with reservation
- ✅ Reservation appears on calendar

### Step 3: Edit Own Reservation

1. Click on created reservation in calendar
2. Click "Edit" button
3. Change end time to 3:30 PM
4. Click "Save"

**Expected Result**:
- ✅ Reservation updated
- ✅ Modification timestamp updated
- ✅ Calendar shows new end time

### Step 4: Attempt to Edit Another User's Reservation

1. Find a reservation created by admin
2. Click on it
3. Attempt to click "Edit" or "Cancel"

**Expected Result**:
- ✅ Edit/Cancel buttons disabled or hidden
- ✅ Attempting API call returns 403 Forbidden
- ✅ Error message: "You can only edit your own reservations"

### Step 5: Cancel Own Reservation

1. Click on own reservation
2. Click "Cancel" button
3. Confirm cancellation

**Expected Result**:
- ✅ Reservation status changes to "Cancelled"
- ✅ Reservation remains in database (audit trail)
- ✅ Reservation displayed as cancelled on calendar (strikethrough or different color)

### Step 6: Browse Published Content

1. Navigate to "Knowledge Base"
2. Verify menu structure appears
3. Click on "Safety" → "Emergency Procedures"
4. Article opens

**Expected Result**:
- ✅ Only published articles visible
- ✅ Only articles within availability period visible
- ✅ Menu items with unpublished articles hidden
- ✅ Content displays with correct formatting (WYSIWYG)
- ✅ Images and YouTube video load correctly

### Step 7: Download Files

1. Navigate to "Knowledge Base" → "Downloads" (if menu item exists)
2. Browse available files
3. Click download on "2025 Safety Manual"

**Expected Result**:
- ✅ File download starts
- ✅ Download counter incremented
- ✅ File downloads correctly

### Step 8: Attempt Admin Actions (Negative Test)

1. Manually navigate to `http://localhost:3000/admin` (if route exists)
2. Attempt to access admin API endpoints via browser dev tools

**Expected Result**:
- ✅ Admin routes redirect to forbidden page or dashboard
- ✅ API returns 403 Forbidden for admin endpoints
- ✅ Error message displayed

### Step 9: Update Preferences

1. Click user menu (top right)
2. Click "Preferences"
3. Change theme to Dark
4. Change sidebar to collapsed
5. Click "Save"
6. Logout and login again

**Expected Result**:
- ✅ Preferences saved
- ✅ Preferences persisted in database
- ✅ Dark theme applied on login
- ✅ Sidebar collapsed state preserved

---

## User Journey 3: Responsive Design Validation

**Goal**: Validate responsive behavior across devices

### Step 1: Desktop View (> 1024px)

1. Resize browser to 1920x1080
2. Navigate through all pages

**Expected Result**:
- ✅ Sidebar visible and expanded by default
- ✅ Calendar shows full week/month grid
- ✅ Content articles display in full width
- ✅ All UI elements fully visible

### Step 2: Tablet View (768px - 1024px)

1. Resize browser to 800px width
2. Navigate through pages

**Expected Result**:
- ✅ Sidebar auto-collapses to icon-only
- ✅ Calendar adjusts grid for smaller space
- ✅ Forms stack vertically
- ✅ Readable on tablet-sized screens

### Step 3: Mobile View (< 768px)

1. Open browser dev tools
2. Switch to mobile device emulation (iPhone 12, 390px)
3. Navigate through pages

**Expected Result**:
- ✅ Sidebar collapsed by default
- ✅ Sidebar icons touchable (tap to expand tooltip)
- ✅ Calendar switches to mobile-optimized view
- ✅ Forms fully functional on mobile
- ✅ All text readable without horizontal scroll
- ✅ Touch targets >= 44px (accessibility)

---

## User Journey 4: Content Lifecycle

**Goal**: Validate complete content approval workflow

### Step 1: Author Creates Draft (as john.doe)

1. Login as `john.doe`
2. Navigate to "Knowledge Base" → "My Articles"
3. Create new article
4. Save as Draft

**Expected Result**:
- ✅ Article saved as Draft
- ✅ Article NOT visible on frontend
- ✅ Article appears in author's draft list

### Step 2: Author Submits for Approval

1. Click "Submit for Approval"

**Expected Result**:
- ✅ Status changes to "Pending Approval"
- ✅ Article sent to admin review queue
- ✅ Author cannot edit while pending

### Step 3: Admin Approves with Availability Period

1. Logout and login as `admin`
2. Navigate to "Admin Panel" → "Pending Articles"
3. Review article
4. Click "Approve"
5. Set availability:
   - Start: Tomorrow
   - End: 7 days from now

**Expected Result**:
- ✅ Status changes to "Published"
- ✅ Article NOT visible today (starts tomorrow)
- ✅ Availability period saved

### Step 4: Article Becomes Available

1. Change system date to tomorrow (or wait)
2. Navigate to Knowledge Base frontend

**Expected Result**:
- ✅ Article NOW visible
- ✅ Article appears in menu
- ✅ Content displays correctly

### Step 5: Article Expires

1. Change system date to 8 days from now (or wait)
2. Navigate to Knowledge Base frontend

**Expected Result**:
- ✅ Article NOT visible (past availability period)
- ✅ Menu item hidden or disabled
- ✅ Direct URL returns 404 or "Not Available"

---

## Performance Validation

### Calendar View Switching Performance

1. Open browser dev tools → Performance tab
2. Start recording
3. Switch from Month → Week → Day → Month
4. Stop recording

**Expected Result**:
- ✅ Each view switch completes in < 200ms
- ✅ No network requests during view switch (client-side only)
- ✅ Smooth animation transitions

### Theme Switching Performance

1. Open dev tools → Performance tab
2. Record theme toggle action

**Expected Result**:
- ✅ Theme switch completes in < 100ms
- ✅ Perceived as instant by user
- ✅ All components update simultaneously

### Initial Page Load Performance

1. Open dev tools → Network tab
2. Hard refresh page (Ctrl+Shift+R)
3. Measure load time

**Expected Result**:
- ✅ Page loads in < 3 seconds on 3G connection
- ✅ First Contentful Paint < 1.5 seconds
- ✅ Time to Interactive < 3 seconds

---

## Validation Checklist

### Authentication & Authorization
- [ ] Admin login successful
- [ ] Normal user login successful
- [ ] Invalid credentials rejected
- [ ] JWT token issued and stored
- [ ] Refresh token rotation works
- [ ] Logout clears tokens
- [ ] Role-based access enforced

### Meeting Room Reservations
- [ ] Create reservation successful
- [ ] Edit own reservation successful
- [ ] Cancel own reservation successful
- [ ] Admin can edit any reservation
- [ ] Conflict detection prevents double-booking
- [ ] Calendar day view works
- [ ] Calendar week view works
- [ ] Calendar month view works
- [ ] View switching < 200ms
- [ ] Filters work correctly

### CMS Content Management
- [ ] Create draft article successful
- [ ] Submit for approval works
- [ ] Admin approval workflow works
- [ ] Availability period enforced
- [ ] Published articles visible to all
- [ ] Draft articles hidden from users
- [ ] Expired articles hidden
- [ ] WYSIWYG editor preserves formatting
- [ ] Image upload works
- [ ] YouTube embed works

### Menu Management
- [ ] Create main menu successful
- [ ] Create sub-menu successful
- [ ] Menu reordering works
- [ ] Menu links to articles
- [ ] External URL links work
- [ ] Hidden menus not visible

### File Management
- [ ] File upload successful
- [ ] File type validation works
- [ ] File size limit enforced (100MB)
- [ ] File download successful
- [ ] Download counter increments
- [ ] File metadata editable

### User Management
- [ ] Create user successful
- [ ] Username uniqueness enforced
- [ ] Email uniqueness enforced
- [ ] Role assignment works
- [ ] User deactivation works
- [ ] Password reset works
- [ ] Preferences save and persist

### Responsive Design
- [ ] Desktop view correct (> 1024px)
- [ ] Tablet view correct (768-1024px)
- [ ] Mobile view correct (< 768px)
- [ ] Sidebar collapse automatic on mobile
- [ ] Touch targets adequate (>= 44px)
- [ ] No horizontal scroll on mobile

### Performance
- [ ] Calendar view switching < 200ms
- [ ] Theme switching < 100ms
- [ ] Initial load < 3 seconds (3G)
- [ ] API responses < 500ms (p95)
- [ ] No memory leaks (extended use)

---

## Troubleshooting

### Services Not Starting

```bash
# Check Docker service status
docker-compose ps

# View logs
docker-compose logs backend
docker-compose logs frontend
docker-compose logs mysql

# Restart services
docker-compose down
docker-compose up -d
```

### Database Connection Issues

```bash
# Check MySQL is ready
docker-compose exec mysql mysql -u root -p -e "SELECT 1"

# Re-run migrations
docker-compose exec backend dotnet ef database drop --force
docker-compose exec backend dotnet ef database update
```

### Frontend Not Loading

```bash
# Check frontend logs
docker-compose logs frontend

# Rebuild frontend
cd frontend
npm install
npm run build
```

---

## Success Criteria

This quickstart validation is considered **PASSED** when:

1. ✅ All 4 user journeys complete without errors
2. ✅ All 60+ checklist items validated
3. ✅ Performance targets met
4. ✅ Responsive design works on all breakpoints
5. ✅ No console errors in browser dev tools
6. ✅ No API errors in backend logs

---

**Validation Date**: _________________
**Validated By**: _________________
**Status**: ⬜ PASS ⬜ FAIL (with issues documented)
