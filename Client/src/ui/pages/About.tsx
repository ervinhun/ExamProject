export default function About() {
    return (
        <div className="container mx-auto px-4 py-8">
            <div className="max-w-4xl mx-auto">
                {/* Header */}
                <div className="mb-8">
                    <h1 className="text-4xl font-bold text-primary mb-3">
                        Loan of Jerne IF Clubhouse
                    </h1>
                    <p className="text-xl text-base-content/70">
                        Information, rules, and pricing for renting the clubhouse
                    </p>
                </div>

                {/* Introduction */}
                <div className="prose prose-lg max-w-none mb-8">
                    <p className="text-base-content/80">
                        The main hall of the Jerne Idrætsforening clubhouse measures
                        <strong> 9 × 12 meters (108 m²)</strong>. We provide tables,
                        chairs, and tableware for approximately <strong>80 people</strong>.
                    </p>
                </div>

                {/* Contact */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">How to Book</h2>
                        <p className="text-base-content/80">
                            Booking of the clubhouse is done by contacting:
                        </p>
                        <div className="mt-3 text-base-content/80">
                            <p><strong>Board member:</strong> Ann Steenholt</p>
                            <p><strong>Email:</strong> asteenholt@gmail.com</p>
                            <p><strong>Mobile:</strong> 29 88 52 80</p>
                        </div>
                    </div>
                </div>

                {/* Conditions */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">Conditions for Loan</h2>
                    <div className="space-y-3 text-base-content/80">
                        <p>
                            The borrower must be a member of Jerne Idrætsforening.
                            Passive members must have been registered during the
                            year of the loan.
                        </p>
                        <p>
                            The clubhouse is normally rented out for entire weekends:
                            <strong> Friday at 12:00 to Sunday at 14:00</strong>.
                        </p>
                        <p>
                            Other days are available by agreement. Opening hours may
                            be extended if the clubhouse is not booked on the following
                            days. This is agreed upon during key handover at the clubhouse
                            with Ann Steenholt.
                        </p>
                        <p className="font-semibold text-warning">
                            Please also read the house rules carefully.
                        </p>
                    </div>
                </div>

                {/* Pricing */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">Prices</h2>
                        <div className="space-y-2 text-base-content/80">
                            <p>
                                <strong>Rental price:</strong> DKK 2,500 (cleaning included)
                            </p>
                            <p>
                                <strong>Deposit:</strong> DKK 600 (refunded after inspection)
                            </p>
                            <p>
                                <strong>Passive membership:</strong> DKK 200 per year
                            </p>
                            <p>
                                Special discounts are available for coaches, leaders,
                                committee members, and board members — please inquire.
                            </p>
                            <p>
                                Tablecloths fitting the tables can be rented for
                                <strong> DKK 30 each (washing included)</strong>.
                            </p>
                        </div>
                    </div>
                </div>

                {/* House Rules */}
                <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-4">House Rules</h2>
                    <div className="space-y-3 text-base-content/80">
                        <p>
                            We value our clubhouse greatly and ask that you take good care
                            of it. Please leave the facilities in the same condition as you
                            would like to receive them.
                        </p>
                        <p>
                            You must provide your own music equipment. The club’s sound
                            system is for internal use only.
                        </p>
                        <p>
                            Do not play loud music. Keep windows and exterior doors closed.
                        </p>
                        <p>
                            Out of consideration for our neighbors, outdoor partying is not
                            allowed — neither on the fields nor in the parking area.
                            Fireworks are strictly prohibited.
                        </p>
                        <p>
                            The use of cinnamon, rice, pepper, or similar items is forbidden
                            both indoors and outdoors.
                        </p>
                        <p>
                            Before the event starts, please check where all equipment and
                            items belong.
                        </p>
                    </div>
                </div>

                {/* After the Event */}
                <div className="card bg-base-200 shadow-md mb-8">
                    <div className="card-body">
                        <h2 className="card-title text-2xl mb-4">After the Event</h2>
                        <div className="space-y-2 text-base-content/80">
                            <p>Tables and chairs must be placed by the end wall facing the parking lot — not in the locker rooms.</p>
                            <p>All floors must be swept.</p>
                            <p>The dishwasher must be emptied and all tableware put away.</p>
                            <p>Kitchen trash bags must be placed in the container outside.</p>
                            <p>Clean up outside areas — remove all glass shards and waste.</p>
                            <p>
                                Do not dispose of empty bottles in the bottle container at
                                night due to noise. Leave them in the kitchen for removal
                                during cleaning.
                            </p>
                        </div>
                    </div>
                </div>

                {/* WiFi & Final Notes */}
                <div className="text-center py-6">
                    <p className="text-base-content/80 mb-2">
                        <strong>WiFi:</strong> Network: WiFimodem-9B5F — Password: vzm3cdznjn
                    </p>
                    <p className="text-lg font-semibold text-error mt-3">
                        Standing on tables and chairs is strictly forbidden.
                    </p>
                    <p className="text-base-content/70 mt-4">
                        Before leaving the clubhouse, everything must be cleaned,
                        tables and chairs returned, and all food and drinks removed.
                    </p>
                </div>
            </div>
        </div>
    );
}
