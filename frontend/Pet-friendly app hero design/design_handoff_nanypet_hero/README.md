# Handoff: NanyPet — Hero (Parallax)

## Overview
Hero section for the **NanyPet** landing page — a pet-sitting & walking marketplace.
Playful, bright, friendly tone. Features a headline + search CTA on the left and a
"pet stage" on the right with floating, layered pet cards (dog, cat, rabbit, bird).
Two parallax effects run at once: **mouse-move depth** (layers shift with the cursor)
and **scroll drift** (layers translate as the page scrolls). Below the hero is a dark
trust strip with key stats.

## About the Design Files
The file in this bundle (`NanyPet Hero.dc.html`) is a **design reference created in HTML** —
a working prototype showing the intended look and behavior. It is **not production code to
copy directly**. The task is to **recreate this design in the NanyPet codebase** using its
existing environment, patterns, and libraries (e.g. React/Next + your CSS approach). If no
front-end environment exists yet, pick the most appropriate one and implement there.

> Note: the prototype uses emoji (🐕 🐈 🐰 🦜 🐾) as stand-ins for pet imagery and icons.
> In production, replace the pet emoji + striped placeholder cards with **real pet photos**,
> and swap decorative emoji for your icon set if you have one.

## Fidelity
**High-fidelity.** Final colors, typography, spacing, radii, shadows, and interactions are
specified below — recreate pixel-accurately using the codebase's libraries. Only the imagery
is placeholder (see note above).

## Screens / Views

### 1. Hero
- **Purpose**: Communicate the value prop and let a visitor start a sitter search by location.
- **Layout**:
  - Full-bleed section, `min-height: 100vh`, horizontal padding `7vw`.
  - Background: `radial-gradient(120% 90% at 80% 0%, #FFE3B0 0%, #FFF6EA 48%, #FFF6EA 100%)`.
  - Top bar (z-index 20): logo left, nav + "Log in" pill right. Vertical padding `30px`.
  - Body: two columns, `display:flex; align-items:center; gap:40px; min-height:calc(100vh - 200px)`.
    - Left column `flex:1 1 50%; max-width:600px` — copy + search.
    - Right column `flex:1 1 50%; position:relative; min-height:560px` — the pet stage (absolutely positioned cards).
- **Components**:
  - **Logo**: 42×42 rounded square (`border-radius:14px`, bg `#FF8A3D`, shadow `0 6px 14px rgba(255,138,61,0.35)`) with 🐾, next to wordmark "Nany**Pet**" — Fredoka 700, 26px, color `#2A2118`, "Pet" in `#FF8A3D`.
  - **Nav links**: Nunito 700, 16px, color `#5C5042`. "Log in" = pill, Nunito 800 15px, white on `#2A2118`, padding `11px 22px`, `border-radius:999px`.
  - **Eyebrow pill**: white bg, `2px solid #FFE0BE`, Nunito 800 14px, color `#FF8A3D`, padding `8px 16px`, `border-radius:999px`, shadow `0 6px 18px rgba(255,138,61,0.10)`. Leading 8px teal dot (`#36C2B4`). Text: "12,000+ trusted local sitters".
  - **Headline**: Fredoka 700, `clamp(44px,5.6vw,82px)`, line-height `0.98`, letter-spacing `-2px`, color `#2A2118`. Text: "Sitters & walkers your pets will **love**" ("love" in `#FF8A3D`). `text-wrap:balance`.
  - **Subcopy**: Nunito 600, 20px, line-height `1.5`, color `#6B5E4E`, max-width `460px`. Text: "Dogs, cats and every furry, feathery or scaly friend in between — book vetted local carers in minutes. Welcome to NanyPet."
  - **Search bar**: white pill, padding `8px`, `border-radius:999px`, shadow `0 16px 40px rgba(42,33,24,0.10)`, max-width `480px`. Contains a 📍 + text input (placeholder "Your neighborhood", Nunito 700 16px, `#2A2118`) and a **Search** button (Fredoka 600 17px, white on `#FF8A3D`, padding `14px 28px`, `border-radius:999px`, shadow `0 8px 18px rgba(255,138,61,0.35)`).
  - **Social proof row**: 3 overlapping 40px avatar circles (bg `#FFD08A`/`#9BE7DC`/`#FFB4C4`, `3px solid #FFF6EA` border, `-12px` overlap) + text "★ 4.9 from 80k+ happy pets" (Nunito 700 15px, `#6B5E4E`; "★ 4.9" 800 `#2A2118`).
  - **Pet cards** (right stage, each absolutely positioned, white `5–6px` border, striped placeholder bg, drop shadow, gentle bob animation):
    - **Dog** (big): 300×300, `border-radius:42px`, `6px` white border, shadow `0 30px 60px rgba(42,33,24,0.18)`. Bottom-left teal tag "Walks daily 🦮" (`#36C2B4`, white text, Fredoka 600 15px). `top:8%; left:18%`.
    - **Cat**: 160×160, `border-radius:34px`. `top:2%; right:2%`.
    - **Rabbit**: 138×138, `border-radius:30px`. `bottom:14%; right:6%`.
    - **Bird**: 108×108, `border-radius:26px`. `bottom:2%; left:6%`.
    - **"Booked!" notification card**: white, `border-radius:18px`, padding `12px 16px`, shadow `0 14px 30px rgba(42,33,24,0.14)`. 38px rounded icon tile (bg `#FFEFD6`) + "Booked!" (Fredoka 600 15px) / "Maya is sitting Luna" (Nunito 700 12px, `#8C7E6C`). `top:46%; left:0`.
  - **Decorative layers**: 3 blurred blob circles (teal/orange/coral, opacity ~0.18–0.22) and 3 🐾 paw prints (opacity ~0.14–0.18) behind content.

### 2. Trust strip
- **Purpose**: Reinforce credibility with headline stats.
- **Layout**: Full-width band, bg `#2A2118`, padding `64px 7vw`. Inner row `display:flex; flex-wrap:wrap; justify-content:space-between; gap:32px; max-width:1100px; margin:0 auto`.
- **Components**: 4 stat blocks, centered. Number = Fredoka 700, 44px, `#FF8A3D`. Label = Nunito 700, 15px, `#C9BDAC`, margin-top `6px`.
  - `12k+` — Vetted local sitters
  - `80k+` — Happy pets cared for
  - `4.9★` — Average sitter rating
  - `24/7` — Support & insurance

## Interactions & Behavior
- **Mouse parallax**: on `mousemove`, compute `mx = clientX - innerWidth/2`, `my = clientY - innerHeight/2`. Each layer has a `depth` value; offset = `mx*depth`, `my*depth`.
- **Scroll parallax**: on `scroll`, read `sy = scrollY`. Each layer has a `scroll` value; vertical offset adds `sy*scroll`.
- **Combined transform** per layer: `translate3d(mx*depth, my*depth + sy*scroll, 0)`, applied inside a `requestAnimationFrame` (one scheduled frame; coalesce mousemove + scroll). Listeners are `passive` for scroll. Clean up on unmount.
- **Depth / scroll values used** (tune to taste — higher = moves more / appears closer):

  | Layer | depth (mouse) | scroll |
  |---|---|---|
  | Teal blob | 0.012 | -0.05 |
  | Orange blob | 0.02 | -0.09 |
  | Coral blob | 0.016 | -0.04 |
  | Paw prints | 0.04–0.08 | 0.04–0.11 |
  | Top bar | 0.01 | 0 |
  | Hero copy column | 0.03 | 0.10 |
  | Dog card | 0.10 | 0.14 |
  | Cat card | 0.16 | 0.05 |
  | Rabbit card | 0.22 | -0.04 |
  | Bird card | 0.30 | 0.18 |
  | "Booked!" card | 0.26 | 0.10 |
  | Trust-strip row | 0.02 | 0.06 |

- **Idle bob animation** on pet cards: `@keyframes bob { 0%,100%{translateY(0)} 50%{translateY(-14px)} }` (and `bobSlow` = -22px). Durations 4–6s, `ease-in-out`, `infinite`, staggered delays. (Optional `wag` rotate keyframe is defined but unused.)
- **Hover states**: not specified in the prototype — apply your design system's defaults for nav links and buttons (e.g. slight darken/scale on the Search and Log-in buttons).
- **Accessibility**: respect `prefers-reduced-motion` — disable parallax + bob for users who request reduced motion.
- **Responsive**: prototype is desktop-first. On narrow screens, stack the two columns (copy above, pet stage below or simplified), reduce headline `clamp`, and consider disabling/softening parallax on touch devices.

## State Management
- Local only: search-input value (controlled input → navigate to results on Search).
- Parallax needs no React state — drive via refs + rAF and write transforms imperatively to avoid re-renders.

## Design Tokens
**Colors**
- Background cream: `#FFF6EA`
- Background gradient highlight: `#FFE3B0`
- Ink / text dark: `#2A2118`
- Body text: `#6B5E4E`; muted: `#5C5042`, `#8C7E6C`, `#9B7B4E`
- Primary orange: `#FF8A3D` (lighter `#FFC36B`, `#FFD08A`, `#FFD9A6`, `#FFCE92`)
- Teal: `#36C2B4` (light `#8FE3D8`, `#9BE7DC`, `#C8EFE8`, `#B5E9E0`)
- Coral/pink: `#FF7E9D` (light `#FFB4C4`, `#FFC0CF`, `#FFD0DC`)
- Trust-strip label: `#C9BDAC`
- Eyebrow border: `#FFE0BE`; icon tile bg `#FFEFD6`

**Typography**
- Display / buttons / numbers: **Fredoka** (weights 400–700)
- Body / nav / labels: **Nunito** (weights 400, 600, 700, 800)
- Monospace (placeholder labels only): system monospace

**Radii**: pills `999px`; cards `42px / 34px / 30px / 26px`; notification `18px`; logo tile `14px`; icon tile `12px`.

**Shadows**
- Card (dog): `0 30px 60px rgba(42,33,24,0.18)`
- Card (cat): `0 22px 44px rgba(42,33,24,0.16)`
- Search bar: `0 16px 40px rgba(42,33,24,0.10)`
- Notification: `0 14px 30px rgba(42,33,24,0.14)`
- Orange button: `0 8px 18px rgba(255,138,61,0.35)`

**Spacing**: section padding `7vw` horizontal; trust strip `64px 7vw`; column gap `40px`.

## Assets
- No raster assets in the prototype — pet visuals and icons are emoji + striped CSS placeholders.
- **For production**: supply 4 pet photos (dog, cat, rabbit, bird/other) for the stage cards, and optional avatar photos for the social-proof row. Replace decorative 🐾 / icons with your icon set if desired.
- Fonts loaded from Google Fonts (Fredoka, Nunito) — self-host or load per your codebase's convention.

## Files
- `NanyPet Hero.dc.html` — the full hero + trust-strip prototype (markup, inline styles, parallax logic). Open in a browser to see the live interaction.
