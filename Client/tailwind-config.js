export default {
    content: ["./src/**/*.{js,ts,jsx,tsx}"],
    plugins: [require("daisyui")],

    daisyui: {
        themes: [
            {
                caramellatte: {
                    "primary": "#F44336",
                    "secondary": "#F44336",
                    "accent": "#F44336",

                    // 👇 Override background color
                    "base-100": "#F44336",

                    // Optional: other background layers
                    "base-200": "#e63d30",
                    "base-300": "#d7392d",
                },
            },

            "light",
            "dark",
        ],
    },
};
